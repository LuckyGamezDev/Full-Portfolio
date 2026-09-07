using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static MainServer.MainServer;
using static ServerFileChunkHandler.ServerFileChunkHandler;

namespace ServerClientDataBase {
    public static class ServerClientDataBase {

        public static Dictionary<int, string> RegisteredClientIdsDictionary { get; private set; } = new Dictionary<int, string>();
        public static Dictionary<(int, string), ClientData> RegisteredClientDataDictionary { get; private set; } = new();
        public static Dictionary<(int, string), ClientConnection> ClientConnectionsDictionary { get; private set; } = new();

        public record ClientRequestInfoMetaData(string Id, int Port, byte? PacketType, byte? PacketCommand, byte[]? PacketData,
            byte[]? PacketDataLength, bool HasExtraData, byte[]? ClientRequestId, byte[]? ServerRequestId);

        private static bool isPingTimerActive = false;
        private static bool hasPingTimerElapsed = false;

        public const int CLIENT_REQUIRED_PING_TIME = 60; // Min needs to be 60 seconds

        public static async Task<ClientConnection?> RegisterClient(string clientId, int clientPort, TcpClient tcpClient) {
            // Send back a response to the client that the server either approves or declines the request
            if (RegisteredClientIdsDictionary.ContainsKey(clientPort) && RegisteredClientIdsDictionary[clientPort] == clientId) {
                // Reject the request because the user is already registered.

                string serverResponds = PacketType.Error.ToString() + ". " + "Access rejected! Reason: User already has access!";
                ServerRequestInfoMetaData request = new ServerRequestInfoMetaData(2, Encoding.UTF8.GetBytes(serverResponds), null, false, null, null);

                await SendServerRequestToNewClientListener(request, tcpClient);

                return null;
            } else { // Accept the request
                ClientConnection connection = new ClientConnection {
                    TcpClient = tcpClient,
                    ClientStream = tcpClient.GetStream(),
                };

                ClientConnectionsDictionary.Add((clientPort, clientId), connection);

                // Add the newly registered user to the dictionary
                RegisteredClientIdsDictionary.Add(clientPort, clientId);

                // Create a new data class for the registered client
                ClientData clientData = new ClientData(clientId, clientPort, ClientStatus.Idle);

                RegisteredClientDataDictionary.Add((clientPort, clientId), clientData);

                // Send back a responds to the client to notify him the request is approved
                Console.WriteLine("Approving client registration");
                string serverResponds = PacketType.ACK.ToString();
                await SendServerMessageToClientListener(serverResponds, connection, null, null);

                Console.WriteLine("ADDED client. Current registered clients: ");

                foreach (var client in RegisteredClientIdsDictionary) {
                    Console.WriteLine($"Client ID: {client.Value}, Client PORT: {client.Key}");
                }

                if (!isPingTimerActive) {
                    _ = StartClientPingCountdownHandler();
                }

                return connection;
            }
        }

        public static void HandleReceivedClientPing(string clientId, int clientPort) {
            DateTime timeStampOfReceivedPing = DateTime.Now;

            // Update the last received ping from the client
            RegisteredClientDataDictionary[(clientPort, clientId)].pingTimerLastStamp = timeStampOfReceivedPing;
        }

        public static Task StartClientPingCountdownHandler() {
            Task.Run(() => {
                Console.WriteLine("Ping timer started!");

                System.Timers.Timer timer = new System.Timers.Timer(CLIENT_REQUIRED_PING_TIME * 1000); // To milliseconds

                timer.Elapsed += On_Timer_Elapsed;

                timer.Start();

                isPingTimerActive = true;

                while (true) {
                    if (hasPingTimerElapsed) {
                        timer.Reset();

                        hasPingTimerElapsed = false;
                    }
                }
            });

            return null;
        }

        public static void Reset(this System.Timers.Timer timer) {
            timer.Stop();
            timer.Start();
        }

        private static async void On_Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
            foreach (var client in RegisteredClientDataDictionary) { // Divide the required ping time by 60 to convert to minutes
                if (DateTime.Now.Minute - client.Value.pingTimerLastStamp.Second > CLIENT_REQUIRED_PING_TIME / 60) { // Check if the client sent a ping on time
                    var serverRequest = new ServerRequestInfoMetaData(6, Encoding.UTF8.GetBytes($"You have been kicked from the server due to inactivity. " +
                        $"Please reconnect to the server to grant access once again."), null, false, null, null);

                    await SendServerRequestToClientListener(serverRequest, ClientConnectionsDictionary[(client.Key)]);

                    RemoveClientFromServer((client.Value.clientPort, client.Value.clientId));
                }
            }
        }

        public static async Task SendServerRequestToNewClientListener(ServerRequestInfoMetaData serverRequestMetaData, TcpClient tcpClient) {
            NetworkStream clientStream = tcpClient.GetStream();

            string serverRequestMetaDataString = JsonSerializer.Serialize<ServerRequestInfoMetaData>(serverRequestMetaData);

            byte[] serverRequestBytes = Encoding.UTF8.GetBytes(serverRequestMetaDataString);
            byte[] serverRequestBytesLength = BitConverter.GetBytes(serverRequestBytes.Length);

            try {
                await clientStream.WriteAsync(serverRequestBytesLength);
                await clientStream.WriteAsync(serverRequestBytes);

                Console.WriteLine("Sent message to new client listener");

                return;
            } catch {
                Console.WriteLine("ERROR: An error occured whilst trying to send message via stream");

                return;
            }
        }

        public static async Task<NetworkStream?> SendServerRequestToClientListener(ServerRequestInfoMetaData serverRequestMetaData, ClientConnection clientConnection) {
            NetworkStream clientStream = clientConnection.ClientStream;

            string serverRequestMetaDataString = JsonSerializer.Serialize<ServerRequestInfoMetaData>(serverRequestMetaData);

            byte[] serverRequestBytes = Encoding.UTF8.GetBytes(serverRequestMetaDataString);
            byte[] serverRequestBytesLength = BitConverter.GetBytes(serverRequestBytes.Length);

            await clientConnection.WriteLock.WaitAsync();

            try {
                await clientStream.WriteAsync(serverRequestBytesLength);
                await clientStream.WriteAsync(serverRequestBytes);

                return clientStream;
            } catch {
                Console.WriteLine("ERROR: An error occured whilst trying to send message via stream");

                return null;
            } finally {
                clientConnection.WriteLock.Release();
            }
        }

        public static async Task<NetworkStream?> SendServerMessageToClientListener(string serverMessage, ClientConnection clientConnection,
            byte[]? clientRequestId, byte[]? serverRequestId, bool isQuestion = false) {
            NetworkStream clientStream = clientConnection.ClientStream;

            byte[] serverMessageBytes = Encoding.UTF8.GetBytes(serverMessage);
            ServerRequestInfoMetaData serverRequestInfoMetaData = new(isQuestion ? (byte)1 : (byte)2, serverMessageBytes, null, false, clientRequestId, serverRequestId);

            string serverRequestMetaDataString = JsonSerializer.Serialize<ServerRequestInfoMetaData>(serverRequestInfoMetaData);

            byte[] serverRequestBytes = Encoding.UTF8.GetBytes(serverRequestMetaDataString);
            byte[] serverRequestBytesLength = BitConverter.GetBytes(serverRequestBytes.Length);

            await clientConnection.WriteLock.WaitAsync();

            try {
                await clientStream.WriteAsync(serverRequestBytesLength);
                await clientStream.WriteAsync(serverRequestBytes);


                Console.WriteLine("Sent message to client, message: " + serverMessage);
                return clientStream;
            } catch {
                Console.WriteLine("ERROR: An error occured whilst trying to send message via stream");

                return null;
            } finally {
                clientConnection.WriteLock.Release();
            }
        }

        public static ClientRequestInfoMetaData? GetClientRequestFromBytes(byte[] bytes) {
            return JsonSerializer.Deserialize<ClientRequestInfoMetaData>(Encoding.UTF8.GetString(bytes));
        }

        private static void RemoveClientFromServer((int clientPort, string clientId) clientInfo) {
            RegisteredClientDataDictionary.Remove((clientInfo.clientPort, clientInfo.clientId));
            RegisteredClientIdsDictionary.Remove(clientInfo.clientPort);
            ClientConnectionsDictionary.Remove((clientInfo.clientPort, clientInfo.clientId));
        }

        public class ClientData {
            public string clientId;
            public int clientPort;

            public ClientStatus status;

            public DateTime pingTimerLastStamp;

            // File related info
            public FileInfoMetaData? fileMetaData;
            public ClientData? fileSendClientReceiverClientData;

            public int sentFileChunksAmount = -1;

            public bool isAllowedToSendFiles = false;
            public string? fileSendClientReceiverClientId;

            public ClientData(string clientId, int clientPort, ClientStatus status) {
                this.clientId = clientId;
                this.clientPort = clientPort;

                this.status = status;
            }
        }

        public enum ClientStatus : byte {
            Idle = 0,
            SendingFile = 1,
            ReceivingFile = 2,
        }
    }

    public class ClientConnection {
        public TcpClient TcpClient { get; init; }
        public NetworkStream ClientStream { get; init; }
        public SemaphoreSlim WriteLock { get; } = new SemaphoreSlim(1, 1);
    }
}
