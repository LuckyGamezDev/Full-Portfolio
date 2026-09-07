using MainClient;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using static ClientFileHandler.ClientFileHandler;
using static ClientToServerHandler.ClientToServerHandler;
using static MainClient.MainClient;


namespace ClientListener {
    public static class ClientListener {


        public static async Task StartAndRunClientListener() {
            NetworkStream serverStream = ServerTcp.GetStream();
            MemoryStream receivedFileChunkBytesStream = new MemoryStream();

            while (true) {
                while (ServerTcp.Connected) {
                    byte[] buffer = new byte[4];
                    await serverStream.ReadExactlyAsync(buffer);

                    byte[] serverRequestMetaDataBytes;
                    serverRequestMetaDataBytes = new byte[BitConverter.ToInt32(buffer)];

                    await serverStream.ReadExactlyAsync(serverRequestMetaDataBytes);

                    ServerRequestInfoMetaData? serverRequestInfoMetaData = DeserializeServerRequestMetaData(serverRequestMetaDataBytes);

                    if (serverRequestInfoMetaData != null) {
                        if (serverRequestInfoMetaData.PacketType != null && serverRequestInfoMetaData.PacketType != 0) {

                            if (serverRequestInfoMetaData.ClientRequestId != null) { // If the metadata has a request ID, assign the data to the tcs
                                Guid requestIdGuid = JsonSerializer.Deserialize<Guid>(Encoding.UTF8.GetString(serverRequestInfoMetaData.ClientRequestId));

                                if (PendingClientRequestsDictionary.TryGetValue(requestIdGuid, out var tcs)) {

                                    tcs.SetResult(serverRequestInfoMetaData);
                                    PendingClientRequestsDictionary.Remove(requestIdGuid);

                                    if (serverRequestInfoMetaData.PacketType == 2) continue;
                                } else {
                                    PrintMessage("Couldn't get the pending request in the dictionary!", ConsoleColor.DarkYellow);
                                }
                            }

                            int fileChunksAmount = -1;
                            switch (serverRequestInfoMetaData.PacketType) {
                                case 1: // Question
                                    await HandleQuestionFromServer(serverRequestInfoMetaData, ServerTcp);

                                    break;
                                case 2: // Message
                                    PrintMessage(ConsoleMessageType[0] + " " + Encoding.UTF8.GetString(serverRequestInfoMetaData.PacketData), ConsoleColor.DarkGreen);

                                    break;
                                case 3: // File Chunk

                                    HandleReceivedFileChunkFromServer(serverRequestInfoMetaData, ref receivedFileChunkBytesStream);

                                    RenderFileTransferVisual();

                                    break;
                                case 4: // File Chunk End
                                    HandleReceivedFileChunkFromServer(serverRequestInfoMetaData, ref receivedFileChunkBytesStream);

                                    // Add the percentage visual logic here.
                                    fileChunksAmount = BitConverter.ToInt32(FileToReceiveMetaData.FileChunksAmount);

                                    RenderFileTransferVisual();

                                    // Call this function twice, but this time with the parameters being null to let the function reset the file chunk count
                                    HandleReceivedFileChunkFromServer(serverRequestInfoMetaData, ref receivedFileChunkBytesStream, true);

                                    // Get the full file from all received bytes
                                    CreateFileFromBytes(receivedFileChunkBytesStream.ToArray());

                                    // Initialize a new network stream for future file chunk receivings
                                    receivedFileChunkBytesStream = new MemoryStream();

                                    // Set the values for sending back to null
                                    clientToSendToId = null;
                                    sendingFilePath = null;

                                    break;
                                case 5: // File meta data
                                    PrintMessage("Received file meta data from server", ConsoleColor.DarkYellow);

                                    HandleReceivedFileMetaData(serverRequestInfoMetaData);

                                    break;
                                case 6: // Kick
                                    PrintMessage(ConsoleMessageType[0] + " " + Encoding.UTF8.GetString(serverRequestInfoMetaData.PacketData), 
                                        ConsoleColor.DarkGreen);

                                    StopServerConnection();

                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static async Task HandleQuestionFromServer(ServerRequestInfoMetaData serverRequestInfoMetaData, TcpClient serverTcp) {
            NetworkStream serverStream = serverTcp.GetStream();

            PrintMessage(ConsoleMessageType[0] + " Question: " + Encoding.UTF8.GetString(serverRequestInfoMetaData.PacketData), ConsoleColor.DarkGreen);

            int respondsNumber;

            isWaitingForQuestionInput = true; // Set the is waiting for question bool to true to activate question input

            respondsNumber = await clientQuestionInputTcs.Task;            

            var clientRequest = new ClientRequestInfoMetaData(ClientId, ClientPort, null, null, BitConverter.GetBytes(respondsNumber),
                null, false, null, serverRequestInfoMetaData.ServerRequestId);

            await SendBytesToServerViaStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clientRequest)), serverStream);
            
            PrintMessage("Sent responds back to server.", ConsoleColor.Blue);
        }
    }
}
