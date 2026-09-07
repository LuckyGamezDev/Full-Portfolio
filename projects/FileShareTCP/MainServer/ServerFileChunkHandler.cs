using ServerClientDataBase;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static MainServer.MainServer;
using static ServerClientDataBase.ServerClientDataBase;

namespace ServerFileChunkHandler {
    public static class ServerFileChunkHandler {

        public const long MAX_ALLOWED_FILE_SIZE_IN_MB = 10000; // ten thousand

        public record FileInfoMetaData(byte[] FileName, byte[] FileSizeMB, byte[] FileChunksAmount);

        public static async Task HandleFileMetaData(FileInfoMetaData? fileMetaData, ClientData clientSenderData, int clientPort, ClientConnection clientSenderConnection) {
            string serverResponds = "";
            ServerRequestInfoMetaData serverRequestInfoMetaData;

            if (fileMetaData == null) {
                Console.WriteLine("The meta data is null");
            }

            string fileName = Encoding.UTF8.GetString(fileMetaData.FileName);

            float fileSize = BitConverter.ToSingle(fileMetaData.FileSizeMB);

            Console.WriteLine($"File: {fileName} ({fileSize})");

            if (fileName == "") {
                serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The given file name isn't valid! FileName: " + fileName;
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);

                return;
            } else if (!Path.HasExtension(fileName)) {
                serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The given file doesn't have an extension!";
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);

                return;
            } else if (fileSize > MAX_ALLOWED_FILE_SIZE_IN_MB) {
                serverResponds = PacketType.Error.ToString() + ". " + $"Request rejected. Reason: The given file size is too big " +
                    $"({fileSize}) Max allowed file size: ({MAX_ALLOWED_FILE_SIZE_IN_MB})";
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);

                return;
            }

            clientSenderData.fileMetaData = fileMetaData;

            // Approve the file meta data
            serverResponds = PacketType.ACK.ToString() + ": File Meta Data";
            await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);
        }

        public static async Task RequestFileTransferApproval(string clientId, int clientPort, ClientConnection clientSenderConnection, 
            string clientToSendToId, ClientConnection clientToSendToConnection, byte[]? clientSenderRequestId) {

            string serverResponds;
            ServerRequestInfoMetaData serverRequestInfoMetaData;

            if (RegisteredClientDataDictionary[(clientPort, clientId)].fileMetaData == null) {
                Console.WriteLine("ERROR: Client sender hasn't sent any file meta data!");

                serverResponds = PacketType.Error.ToString() + ". " + "Client hasn't sent any file meta data. Please send file meta data first before " +
                    "making a transfer request!";
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, clientSenderRequestId, null);

                return;
            }

            int clientToSendToPort = -1;
            try {
                clientToSendToPort = RegisteredClientIdsDictionary.FirstOrDefault(x => x.Value == clientToSendToId).Key;
            } catch {
                // Reject the request since the user already has approval to send files
                serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user you want to send to isn't registered on this server!";
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, clientSenderRequestId, null);

                return;
            }

            Guid serverRequestId = Guid.NewGuid(); // Create a GUID for the responds the server expects from the client receiver
            byte[] serverRequestIdBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(serverRequestId));

            if (RegisteredClientIdsDictionary.ContainsKey(clientToSendToPort) && RegisteredClientIdsDictionary[clientToSendToPort] == clientToSendToId) {
                ClientData clientSenderData = RegisteredClientDataDictionary[(clientPort, clientId)];;

                if (clientSenderData.isAllowedToSendFiles) {
                    // Reject the request since the user already has approval to send files
                    serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user already has permission to send files.";
                    await SendServerMessageToClientListener(serverResponds, clientSenderConnection, clientSenderRequestId, null);

                    return;
                }

                serverResponds = $"A user with the ID {clientId} wants to send a file to you ({BitConverter.ToSingle(clientSenderData.fileMetaData.FileSizeMB)}MB). " +
                    $"Do you accept? yes = 1 | no = 0";
                NetworkStream? clientToSendToStream = await SendServerMessageToClientListener(serverResponds, clientToSendToConnection, null, serverRequestIdBytes, true);

                Console.WriteLine("Sent the question to the receiver client");

                if (clientToSendToStream == null) { // If the stream is null, let the sender know that the file send didn't succeed
                    Console.WriteLine("ERROR: Failed to connect to client receiver!");

                    serverResponds = $"{PacketType.Error}: Failed to connect to client receiver!";
                    await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, serverRequestIdBytes);

                    return;
                }

                Console.WriteLine("Waiting for the client responds...");

                // Expect a responds from the other client (0 or 1)
                int respondsNumber;
                try {
                    var tcs = new TaskCompletionSource<ClientRequestInfoMetaData>(TaskCreationOptions.RunContinuationsAsynchronously);

                    ServerTCSsDictionary[serverRequestId] = tcs;

                    ClientRequestInfoMetaData? clientRequest = null;
                    int timeoutSecondsAmount = 20;
                    try {
                        clientRequest = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(timeoutSecondsAmount));
                    } catch {
                        Console.WriteLine("Client receiver timed out!");

                        ServerTCSsDictionary.Remove(serverRequestId);

                        serverResponds = PacketType.Error.ToString() + ". " + "There was an issue on the client receiver's end.";
                        await SendServerMessageToClientListener(serverResponds, clientSenderConnection, clientSenderRequestId, serverRequestIdBytes);

                        return;
                    }

                    Console.WriteLine("Got the tcs");

                    respondsNumber = BitConverter.ToInt32(clientRequest.PacketData);
                } catch {
                    Console.WriteLine("ERROR with client receiver, breaking connection!");

                    serverResponds = PacketType.Error.ToString() + ". " + "There was an issue on the client receiver's end.";
                    await SendServerMessageToClientListener(serverResponds, clientSenderConnection, clientSenderRequestId, serverRequestIdBytes);

                    return;
                }

                Console.WriteLine("Comparing responds numbers");
                if (respondsNumber == 1) { // Request Accepted
                                           // Update the permission status
                    ClientData clientSenderClientData = RegisteredClientDataDictionary[(clientPort, clientId)];

                    clientSenderClientData.isAllowedToSendFiles = true;
                    clientSenderClientData.fileSendClientReceiverClientId = clientToSendToId;
                    clientSenderClientData.fileSendClientReceiverClientData =
                        RegisteredClientDataDictionary[(clientToSendToPort, clientToSendToId)];

                    // Set the sent file chunks amount to 0, indicating that the client IS sending a file, but hasn't started yet
                    clientSenderClientData.sentFileChunksAmount = 0;

                    // Approve file transfer
                    serverResponds = PacketType.ACK.ToString();
                    await SendServerMessageToClientListener(serverResponds, clientSenderConnection, clientSenderRequestId, null);

                } else { // Request Rejected
                         // Tell the client sender that the receiver doesn't approve the request
                    serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user you want to send Doesn't accept the file send request!";
                    await SendServerMessageToClientListener(serverResponds, clientSenderConnection, clientSenderRequestId, null);
                }

            } else {
                // Reject the request because the client the sender wants to send a file to isn't registered
                serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user you want to send to isn't registered on this server!";
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, clientSenderRequestId, null);
            }

            Console.WriteLine("Proccessed the received client responds");
        }

        public static async Task HandleFileChunkPacket(ClientRequestInfoMetaData clientRequest, ClientData clientData, ClientConnection clientSenderConnection,
            byte[] extraDataBytes, int clientPort, bool isFirstFileChunk = false, bool isLastFileChunk = false) {

            string serverResponds = "";
            ServerRequestInfoMetaData request;

            string clientToSendToId;
            int clientToSendToPort;
            if (clientRequest.PacketData == null || clientRequest.PacketDataLength == null) {
                Console.WriteLine("ERROR: The sent data is null!");

                // Reject request because the user isn't registered to this server
                serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user didn't send any file bytes data!";
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);

                return;
            } else if (clientData == null) {
                Console.WriteLine("ERROR: The user isn't registered or does not have a saved data!");

                // Reject request because the user isn't registered to this server
                serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user isn't registered or does not have a saved data!";
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);

                return;
            }

            clientToSendToId = Encoding.UTF8.GetString(extraDataBytes);
            clientToSendToPort = RegisteredClientIdsDictionary.FirstOrDefault(x => x.Value == clientToSendToId).Key;

            // Make sure the client ID of the sender isn't the same as the client receiver's ID
            if (clientToSendToId != clientData.fileSendClientReceiverClientId) {
                Console.WriteLine("ERROR: The client sender wants to send to a different client than he is allowed to.");

                // Reject request because the user isn't registered to this server
                serverResponds = PacketType.Error.ToString() + ". " +
                    "Request rejected. Reason: The user wants to send to a client of which is he isn't allowed to send to!";
                await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);

                return;
            }

            int packetDataLength = BitConverter.ToInt32(clientRequest.PacketDataLength);
            await ProcessFileChunk(clientRequest.Id, clientToSendToId, clientPort, clientRequest.PacketData, packetDataLength, 
                clientSenderConnection, ClientConnectionsDictionary[(clientToSendToPort, clientToSendToId)], isFirstFileChunk, isLastFileChunk);
        }

        public static async Task ProcessFileChunk(string clientSenderId, string clientToSendToId, int clientSenderPort, byte[] fileChunkRawBytes,
            int fileChunkLength, ClientConnection clientSenderConnection, ClientConnection clientToSendToConnection, bool isFirstFileChunk = false, bool hasReceivedLastChunk = false) {
            if (RegisteredClientDataDictionary[(clientSenderPort, clientSenderId)].isAllowedToSendFiles == true) {

                if (RegisteredClientIdsDictionary.ContainsValue(clientToSendToId)) { // Check if the client that the user wants to send to is registered
                    int clientToSendToPort = RegisteredClientIdsDictionary.FirstOrDefault(x => x.Value == clientToSendToId).Key;

                    ClientData clientToSendToData = RegisteredClientDataDictionary[(clientToSendToPort, clientToSendToId)];
                    ClientData clientSenderData = RegisteredClientDataDictionary[(clientSenderPort, clientSenderId)];

                    if (isFirstFileChunk) { // Send the client receiver the full file meta data for the client to review and access
                        Console.WriteLine("Is the first chunk");

                        byte[] fileChunkLengthBytes = BitConverter.GetBytes(fileChunkLength);
                        string fileMetaDataString = JsonSerializer.Serialize<FileInfoMetaData>(clientSenderData.fileMetaData);

                        // Prepare the file meta data
                        ServerRequestInfoMetaData fileChunkMetaData = new ServerRequestInfoMetaData(5, Encoding.UTF8.GetBytes(fileMetaDataString),
                            fileChunkLengthBytes, false, null, null);

                        Console.WriteLine("Sending file meta data to client receiver because this is the first chunk");
                        // Send the file meta data
                        await SendServerRequestToClientListener(fileChunkMetaData, clientToSendToConnection);
                    } else {
                        Console.WriteLine("Is not the first chunk");
                    }

                    if (!hasReceivedLastChunk) {
                        if (clientToSendToData.status != ClientStatus.ReceivingFile) // If the client status is not yet set to receiving file, set it
                            clientToSendToData.status = ClientStatus.ReceivingFile;

                        if (fileChunkRawBytes.Length <= allowedFileChunkBuffer) {
                            byte[] fileChunkLengthBytes = BitConverter.GetBytes(fileChunkLength);

                            ServerRequestInfoMetaData fileChunkMetaData = new ServerRequestInfoMetaData(3, fileChunkRawBytes, fileChunkLengthBytes, false, null, null);

                            await SendServerRequestToClientListener(fileChunkMetaData, clientToSendToConnection);

                            clientSenderData.sentFileChunksAmount++;
                        } else {
                            // Throw an error because this function should never receive more than the maximum buffer
                            Console.WriteLine("ERROR: The sent file chunk bytes exceed the maximum buffer, this should never happen!");
                        }

                        // Change the client's status to sending files if not yet updated
                        if (RegisteredClientDataDictionary.ContainsKey((clientSenderPort, clientSenderId))) {
                            RegisteredClientDataDictionary[(clientSenderPort, clientSenderId)].status = ClientStatus.SendingFile;
                        } else {
                            Console.WriteLine("ERROR: The sender doesn't have a registered client data profile. The sender is invalid!");

                            // Reject the request because the sender doesn't have any registered data
                            string serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user doesn't have any user data stored on the server, this user is invalid!";
                            await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);
                        }
                    } else if (hasReceivedLastChunk) { // Tell the client to send to (receiver) that he has received all the data (FileEnd)
                        byte[] fileChunkLengthBytes = BitConverter.GetBytes(fileChunkLength);

                        // Prepare the last file chunk
                        ServerRequestInfoMetaData fileChunk = new ServerRequestInfoMetaData(4, fileChunkRawBytes, fileChunkLengthBytes, false, null, null);

                        // Send the last file chunk
                        await SendServerRequestToClientListener(fileChunk, clientToSendToConnection);

                        clientToSendToData.sentFileChunksAmount++;

                        // Tell the client sender that the file send was succesful.
                        string serverResponds = "Sent all file chunks to client succesfully!";
                        await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);

                        clientSenderData.status = ClientStatus.Idle;
                        clientSenderData.sentFileChunksAmount = -1; // Set to -1, indicating that the sender is NOT currently sending anything

                        clientToSendToData.status = ClientStatus.Idle;

                        // Revoke the permission to send files because sending is done with the specific client requested
                        RegisteredClientDataDictionary[(clientSenderPort, clientSenderId)].isAllowedToSendFiles = false;
                        RegisteredClientDataDictionary[(clientSenderPort, clientSenderId)].fileSendClientReceiverClientId = null;
                    } 
                } else {
                    // Reject the request because the user that the sender wants to send to isn't registered to this server
                    string serverResponds = PacketType.Error.ToString() + ". " + "Request rejected. Reason: The user of which you want to send to isn't registered to this server.";
                    await SendServerMessageToClientListener(serverResponds, clientSenderConnection, null, null);
                }
            }

        }
    }
}