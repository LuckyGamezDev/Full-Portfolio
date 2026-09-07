using ServerClientDataBase;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static ServerClientDataBase.ServerClientDataBase;
using static ServerFileChunkHandler.ServerFileChunkHandler;

namespace MainServer {
    public static class MainServer {
        private static IPHostEntry? ipHostEntry = null;
        private static IPAddress? ipAddress = null;

        public static Dictionary<Guid, TaskCompletionSource<ClientRequestInfoMetaData>> ServerTCSsDictionary { get; private set; } = new();

        public static readonly int allowedFileChunkBuffer = 2048;

        public const int SERVER_PORT = 7777;

        public record ServerRequestInfoMetaData(byte? PacketType, byte[]? PacketData, byte[]? PacketDataLength, bool HasExtraData, byte[]? ClientRequestId, byte[]? ServerRequestId);


        private static async Task Main(string[] args) {
            await StartServer();
        }

        public static async Task StartServer() {
            ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHostEntry.AddressList[0];

            Console.WriteLine(Dns.GetHostName());
            Console.WriteLine("Server Started!");

            await StartListenerForNewClient(SERVER_PORT);
        }


        private static async Task StartListenerForNewClient(int serverPort) {
            var listener = new TcpListener(IPAddress.Any, serverPort);

            listener.Start();

            while (true) {
                Console.WriteLine("Waiting for a new client to connect...");

                var client = await listener.AcceptTcpClientAsync();
                var stream = client.GetStream();

                Console.WriteLine("Connected to new client!");

                byte[] buffer = new byte[4];
                await stream.ReadExactlyAsync(buffer);

                byte[] requestBytes = new byte[BitConverter.ToInt32(buffer)];
                await stream.ReadExactlyAsync(requestBytes);

                var clientRequest = GetClientRequestFromBytes(requestBytes);

                ClientConnection? connection = await HandleClientRegistration(clientRequest, client, clientRequest.ClientRequestId);

                if (connection != null) {
                    _ = StartClientRequestsListener(connection, clientRequest.Id); // Start the listener future client requests from this client
                } else {
                    Console.WriteLine("[ERROR] client connetion is null, can't continue!");

                    break;
                }
            }
        }

        private static async Task StartClientRequestsListener(ClientConnection clientConnection, string clientId) {
            Console.WriteLine("Started client listener for a client");

            NetworkStream stream = clientConnection.ClientStream;

            byte[] buffer;
            byte[] requestBytes;
            while (clientConnection.TcpClient.Connected) {
                Console.WriteLine($"Waiting for request on client ID {clientId}...");

                buffer = new byte[4];
                await stream.ReadExactlyAsync(buffer);

                Console.WriteLine("Read length bytes");

                requestBytes = new byte[BitConverter.ToInt32(buffer)];
                await stream.ReadExactlyAsync(requestBytes);

                Console.WriteLine("Read data bytes");

                var clientRequest = GetClientRequestFromBytes(requestBytes);

                if (clientRequest.ServerRequestId != null) {
                    var requestId = JsonSerializer.Deserialize<Guid>(clientRequest.ServerRequestId);

                    // Search for an active TCS that is waiting for a value
                    if (ServerTCSsDictionary.TryGetValue(requestId, out var tcs)) {
                        tcs.SetResult(clientRequest);
                        ServerTCSsDictionary.Remove(requestId);

                        continue;
                    } else {
                        Console.WriteLine("Couldn't find any active TCSs on this request ID!");

                        string serverMessage = PacketType.Error.ToString() + ". " + "The server request ID sent in the client request was not valid. " +
                            "This may have been a problem on the server side or the client timed out!";

                        await SendServerMessageToClientListener(serverMessage, clientConnection, null, null);

                        continue;
                    }
                }

                if (clientRequest.HasExtraData) {
                    buffer = new byte[4];
                    await stream.ReadExactlyAsync(buffer);

                    requestBytes = new byte[BitConverter.ToInt32(buffer)];
                    await stream.ReadExactlyAsync(requestBytes);

                    _ = HandleReceivedClientRequest(clientRequest, requestBytes, clientConnection);
                } else {
                    _ = HandleReceivedClientRequest(clientRequest, null, clientConnection);
                }
            }
        }

        private static async Task<ClientConnection?> HandleClientRegistration(ClientRequestInfoMetaData clientRequest, TcpClient tcpClient, byte[]? requestId) {
            int clientPort;

            var stream = tcpClient.GetStream();

            if (clientRequest != null) {
                clientPort = clientRequest.Port;
            } else {
                Console.WriteLine("Client meta data read was unsuccesful, closing connection!");

                return null;
            }

            ClientData? clientData = null;

            if (RegisteredClientDataDictionary.ContainsKey((clientPort, clientRequest.Id))) {
                clientData = RegisteredClientDataDictionary[(clientPort, clientRequest.Id)];
            }

            string serverResponds;
            ServerRequestInfoMetaData request;
            if (clientRequest.PacketType != 1) {
                // Reject the request because the user isn't registered to this server
                serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user isn't allowed to make this request without being registered.";
                request = new ServerRequestInfoMetaData(2, Encoding.UTF8.GetBytes(serverResponds), null, false, requestId, null);

                await SendServerRequestToNewClientListener(request, tcpClient);

                Console.WriteLine("Client wasn't registered, making it unauthorized for it to make this request.");

                return null;
            }

            ClientConnection? connection;
            connection = await RegisterClient(clientRequest.Id, clientRequest.Port, tcpClient);

            return connection;
        }

        private static async Task HandleReceivedClientRequest(ClientRequestInfoMetaData clientRequest, byte[]? clientExtraDataBytes, ClientConnection clientConnection) {
            int clientPort;

            var stream = clientConnection.ClientStream;

            if (clientRequest != null) {
                clientPort = clientRequest.Port;
            } else {
                Console.WriteLine("Client meta data read was unsuccesfully, closing connection!");

                return;
            }

            ClientData? clientData = null;

            if (RegisteredClientDataDictionary.ContainsKey((clientPort, clientRequest.Id))) {
                clientData = RegisteredClientDataDictionary[(clientPort, clientRequest.Id)];
            }

            byte[]? extraDataBytes = clientExtraDataBytes;

            string serverResponds = "";
            ServerRequestInfoMetaData request;

            if (clientRequest.PacketType != null || clientRequest.PacketType != 0) {
                if (!RegisteredClientDataDictionary.ContainsKey((clientPort, clientRequest.Id))) {
                    // Reject the request because the user isn't registered to this server
                    serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user isn't registered to this server.";
                    await SendServerMessageToClientListener(serverResponds, clientConnection, clientRequest.ClientRequestId, null);

                    Console.WriteLine("Client wasn't registered, making it unauthorized for it to make this request.");

                    return;
                }

                switch (clientRequest.PacketType) {
                    case 1: // Handshake
                        break;
                    case 2: // File Chunk
                        bool isFirstFileChunk = clientData.sentFileChunksAmount == 0;

                        Console.WriteLine("Is first file?: " + isFirstFileChunk);
                        await HandleFileChunkPacket(clientRequest, clientData, clientConnection, extraDataBytes, clientPort, isFirstFileChunk);

                        break;
                    case 3: // File End
                        await HandleFileChunkPacket(clientRequest, clientData, clientConnection, extraDataBytes, clientPort, false, true);

                        break;
                    case 4: // File Meta Data
                        FileInfoMetaData? fileMetaData = JsonSerializer.Deserialize<FileInfoMetaData>(Encoding.UTF8.GetString(clientRequest.PacketData));

                        await HandleFileMetaData(fileMetaData, clientData, clientPort, clientConnection);

                        break;
                    case 5:

                        break;
                    case 6:

                        break;
                    case 7: // Ping
                        HandleReceivedClientPing(clientRequest.Id, clientPort);

                        break;
                }
            }

            if (clientRequest?.PacketCommand != null || clientRequest?.PacketCommand != 0) {
                switch (clientRequest?.PacketCommand) {
                    case 1: // Request file transfer
                        string clientToSendToId = "";
                        int clientToSendToPort;

                        if (extraDataBytes != null) {
                            clientToSendToId = Encoding.UTF8.GetString(extraDataBytes);
                            clientToSendToPort = RegisteredClientIdsDictionary.FirstOrDefault(x => x.Value == clientToSendToId).Key;
                        } else {
                            Console.WriteLine("ERROR: The expected extra data is null!");

                            // Reject request because the user didn't send the addiquate data
                            serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user didn't send any required extra data!";
                            await SendServerMessageToClientListener(serverResponds, clientConnection, clientRequest.ClientRequestId, null);

                            break;
                        }

                        if (clientToSendToId == clientRequest.Id) {
                            Console.WriteLine("ERROR: The ID the user wants to send to is invalid!");

                            // Reject request because the user isn't registered to this server
                            serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The sender wants to send to himself which isn't allowed!";
                            await SendServerMessageToClientListener(serverResponds, clientConnection, clientRequest.ClientRequestId, null);

                            break;
                        }

                        try {
                            await RequestFileTransferApproval(clientRequest.Id, clientPort, clientConnection,
                                clientToSendToId, ClientConnectionsDictionary[(clientToSendToPort, clientToSendToId)], clientRequest.ClientRequestId);
                        } catch (Exception e) {
                            // Reject the request since the user already has approval to send files
                            serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user you want to send to isn't registered on this server!";
                            await SendServerMessageToClientListener(serverResponds, clientConnection, clientRequest.ClientRequestId, null);

                            Console.WriteLine(e);

                            return;
                        }

                        break;
                    case 7: // Get the current allowed file chunk buffer
                        serverResponds = allowedFileChunkBuffer.ToString();
                        await SendServerMessageToClientListener(serverResponds, clientConnection, clientRequest.ClientRequestId, null);

                        break;
                    case 8: // Get the required ping duration
                        serverResponds = CLIENT_REQUIRED_PING_TIME.ToString();
                        await SendServerMessageToClientListener(serverResponds, clientConnection, clientRequest.ClientRequestId, null);

                        break;
                }
            }
        }

        public static string GetLocalIPv4() {
            foreach (var netInterface in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
                if (netInterface.AddressFamily == AddressFamily.InterNetwork) {
                    return netInterface.ToString();
                }
            }

            throw new Exception("No network adapters with an IPv4 address found.");
        }

    public enum PacketCommand : byte {
            RequestFileTransferApproval = 1,
            GetAllowedFileChunkBuffer = 7
        }

        public enum PacketType : byte {
            HandShake = 1,
            FileChunk = 2,
            FileEnd = 3,
            FileMetaData = 4,
            ACK = 5,
            Error = 6,
            Ping = 7,
        }
    }
}

// To do:
// Bug fixes:

// Fix at the end:
// Make sure there's no way that the server and/or client crash when an error occurs on one side

// Polish:
