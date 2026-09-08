using MainClient;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static ClientToServerHandler.ClientToServerHandler;
using static MainClient.MainClient;

namespace ClientFileHandler {
    public static class ClientFileHandler {

        public const int FILE_SIZE_TO_MB_DIVISION = 1000000; // A million

        public static string? sendingFilePath = null;

        private static int? percentageVisualConsoleCursorTopIndex = null;

        public static FileInfoMetaData? FileToReceiveMetaData { get; private set; }
        public static int FileToReceiveReceivedChunksAmount { get ; private set; }

        public record FileInfoMetaData(byte[] FileName, byte[] FileSizeMB, byte[] FileChunksAmount);

        public static async Task AskServerForFileTransferPermission() {
            PrintMessage(ConsoleMessageType[1] + " Please enter the coresponding ID for the client you want to send to:", ConsoleColor.Cyan);

            string? clientToSendFilesToId;

            while (true) {
                clientToSendFilesToId = Console.ReadLine();

                if (clientToSendFilesToId != null && clientToSendFilesToId != "") {
                    break;
                }

                PrintMessage(ConsoleMessageType[1] + " This ID was not valid, please try again: ", ConsoleColor.Cyan);
            }

            PrintMessage(ConsoleMessageType[1] + " File path:", ConsoleColor.Cyan);

            string? filePath = "";
            while (true) {
                filePath = Console.ReadLine();

                if (filePath != null && filePath != "") {
                    break;
                }

                PrintMessage(ConsoleMessageType[1] + "This file path was not valid, please try again:", ConsoleColor.Cyan);
            }

            sendingFilePath = filePath;

            int allowedFileChunkBuffer = -1;
            try {
                allowedFileChunkBuffer = GetCurrentAllowedFileChunkBuffer().Result;
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            bool result = await SendFileMetaDataToServer(sendingFilePath, allowedFileChunkBuffer);

            if (!result)
                return;

            // Generate a request id
            Guid requestId = Guid.NewGuid();
            string requestIdString = JsonSerializer.Serialize(requestId);
            byte[] requestIdStringBytes = Encoding.UTF8.GetBytes(requestIdString);

            // Create a server request to send to the server
            ClientRequestInfoMetaData clientServerRequest = new ClientRequestInfoMetaData(ClientId, ClientPort, null, 1,
                null, null, true, requestIdStringBytes, null);

            EncodeClientRequestInfoMetaData(clientServerRequest, out byte[] dataBytes, out byte[] dataBytesLength);

            NetworkStream stream = ServerTcp.GetStream();

            var tcs = new TaskCompletionSource<ServerRequestInfoMetaData>(TaskCreationOptions.RunContinuationsAsynchronously);

            PendingClientRequestsDictionary[requestId] = tcs;

            // Send the request
            await SendBytesToServerViaStream(dataBytes, dataBytesLength, stream);

            // Send the required client to send to ID (Extra data)
            await SendBytesToServerViaStream(Encoding.UTF8.GetBytes(clientToSendFilesToId), stream);
            
            PrintMessage(ConsoleMessageType[1] + " Waiting for server file transfer approval...", ConsoleColor.Cyan);

            var serverRequest = await tcs.Task;

            string serverResponds = Encoding.UTF8.GetString(serverRequest.PacketData);

            PrintMessage(ConsoleMessageType[0] + " " + serverResponds + ": File Transfer", ConsoleColor.DarkGreen);

            if (serverResponds == "ACK") {
                clientToSendToId = clientToSendFilesToId;
            }
        }

        public static async Task<bool> SendFileToOtherClient(int allowedBufferSize) {
            if (clientToSendToId == null) {

                PrintMessage(ConsoleMessageType[1] + "User is not allowed to send files yet! Please first make a request to the server.", ConsoleColor.Cyan);

                return false;
            } else if (sendingFilePath == null) {
                PrintMessage(ConsoleMessageType[1] + "Please ask permission to send files to the server and specify a file path!", ConsoleColor.Cyan);

                return false;
            }

            ClientRequestInfoMetaData clientServerRequest;
            NetworkStream stream;

            byte[]? fileBytes = null;
            try {
                fileBytes = File.ReadAllBytes(sendingFilePath);
            } catch (Exception excetpion) {
                Console.WriteLine($"{excetpion.Message}");
            }

            List<byte[]> fileByteChunksArray = GetChunkByteArray(fileBytes, allowedBufferSize);

            byte[] clientToSendToIdBytes = Encoding.UTF8.GetBytes(clientToSendToId);
            byte[] clientToSendToIdBytesLength = BitConverter.GetBytes(clientToSendToId.Length);

            PrintMessage(ConsoleMessageType[1] + $" Sending {Path.GetFileName(sendingFilePath)} ({fileBytes.Length / FILE_SIZE_TO_MB_DIVISION}MB)", 
                ConsoleColor.Cyan);

            int index = 0;
            foreach (byte[] chunk in fileByteChunksArray) { // Is this possible, awaiting within in a foreach loop? Else do it in a while loop which will work.
                stream = ServerTcp.GetStream();

                byte[] chunkLength = BitConverter.GetBytes(chunk.Length);
                if (index == fileByteChunksArray.Count - 1) { // If this is the last file chunk, let the server know by changing the packet type to 'file end'
                    clientServerRequest = new ClientRequestInfoMetaData(ClientId, ClientPort, 3, null, chunk, chunkLength, true, null, null);

                    PrintMessage(ConsoleMessageType[1] + " Sent last file chunk!", ConsoleColor.Cyan);
                } else {
                    clientServerRequest = new ClientRequestInfoMetaData(ClientId, ClientPort, 2, null, chunk, chunkLength, true, null, null);
                }

                EncodeClientRequestInfoMetaData(clientServerRequest, out byte[] dataBytes, out byte[] dataBytesLength);

                // Send the request
                await SendBytesToServerViaStream(dataBytes, dataBytesLength, stream);

                // Send the client to send to id (Extra data)
                await SendBytesToServerViaStream(clientToSendToIdBytes, clientToSendToIdBytesLength, stream);

                index++;
            }

            sendingFilePath = null;

            return true;
        }

        public static async Task<bool> SendFileMetaDataToServer(string filePath, int fileChunkBuffer) {
            byte[] fileBytes;

            try {
                fileBytes = File.ReadAllBytes(filePath);

            } catch {
                PrintMessage(ConsoleMessageType[1] + " File doesn't exist in the given file path!", ConsoleColor.Cyan);

                return false;
            }

            byte[] fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(filePath));
            byte[] fileSizeBytes = BitConverter.GetBytes(MathF.Round((float)fileBytes.Length / FILE_SIZE_TO_MB_DIVISION, 2));

            PrintMessage(ConsoleMessageType[1] + $" File size: {MathF.Round((float)fileBytes.Length / FILE_SIZE_TO_MB_DIVISION, 2)}MB", ConsoleColor.Cyan);

            byte[] fileChunksAmountBytes = BitConverter.GetBytes(GetChunkByteArray(fileBytes, fileChunkBuffer).Count);
            FileInfoMetaData fileMetaData = new FileInfoMetaData(fileNameBytes, fileSizeBytes, fileChunksAmountBytes);

            byte[] fileMetaDataBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(fileMetaData));
            byte[] fileMetaDataBytesLength = BitConverter.GetBytes(fileMetaDataBytes.Length);

            ClientRequestInfoMetaData clientServerRequest = new ClientRequestInfoMetaData(ClientId, ClientPort, 4, null,
                fileMetaDataBytes, fileMetaDataBytesLength, false, null, null);

            EncodeClientRequestInfoMetaData(clientServerRequest, out byte[] clientRequestBytes, out byte[] clientRequestBytesLength);

            NetworkStream stream = ServerTcp.GetStream();

            // Send the file meta data
            await SendBytesToServerViaStream(clientRequestBytes, clientRequestBytesLength, stream);

            return true;
        }

        public static void HandleReceivedFileMetaData(ServerRequestInfoMetaData serverRequestInfoMetaData) {
            string deserializedFileMetaDataString = Encoding.UTF8.GetString(serverRequestInfoMetaData.PacketData);
            FileInfoMetaData? deserializedFileMetaData = JsonSerializer.Deserialize<FileInfoMetaData>(deserializedFileMetaDataString);

            if (deserializedFileMetaData == null) {
                throw new Exception("ERROR: The file info meta data is null. This is a server-side problem!");
            }

            FileToReceiveMetaData = deserializedFileMetaData;
        }

        public static void HandleReceivedFileChunkFromServer(ServerRequestInfoMetaData? serverRequestInfoMetaData, ref MemoryStream? receivedBytesMemoryStream, bool isLastChunk = false) {
            if (serverRequestInfoMetaData == null || receivedBytesMemoryStream == null) { 
                if (isLastChunk) {
                    FileToReceiveReceivedChunksAmount = 0;

                    return;
                }

                // If the program is continued despite the exception, reset this value to not instantly break everything
                FileToReceiveReceivedChunksAmount = 0;

                throw new Exception("One or more of the given parameters are null!");
            }

            receivedBytesMemoryStream.Write(serverRequestInfoMetaData.PacketData, 0, serverRequestInfoMetaData.PacketData.Length);

            if (isLastChunk) FileToReceiveReceivedChunksAmount = 0;
            else FileToReceiveReceivedChunksAmount++;
        }

        public static async Task<int> GetCurrentAllowedFileChunkBuffer() {
            var stream = ServerTcp.GetStream();

            Guid requestId = Guid.NewGuid();
            string requestIdString = JsonSerializer.Serialize(requestId);
            byte[] requestIdStringBytes = Encoding.UTF8.GetBytes(requestIdString);

            // Create a server request to send to the server
            ClientRequestInfoMetaData clientServerRequest = new ClientRequestInfoMetaData(ClientId, ClientPort, null, 7,
                null, null, false, requestIdStringBytes, null);

            EncodeClientRequestInfoMetaData(clientServerRequest, out byte[] dataBytes, out byte[] dataBytesLength);

            // Send the request
            await SendBytesToServerViaStream(dataBytes, dataBytesLength, stream);

            var tcs = new TaskCompletionSource<ServerRequestInfoMetaData>(TaskCreationOptions.RunContinuationsAsynchronously);

            PendingClientRequestsDictionary[requestId] = tcs;

            var serverRequest = await tcs.Task;
            return int.Parse(serverRequest.PacketData);
        }

        public static List<byte[]> GetChunkByteArray(byte[] bytesArray, int chunkSize = 64) {
            List<byte[]> chunksList = new List<byte[]>();

            if (bytesArray == null || bytesArray.Length == 0) {
                // Return an empty list
                return chunksList;
            }

            int chunksAmount = (bytesArray.Length + chunkSize - 1) / chunkSize;

            for (int i = 0; i < chunksAmount; i++) {
                int startIndex = i * chunkSize;
                int currentChunkLength = Math.Min(chunkSize, bytesArray.Length - startIndex);
                byte[] chunk = new byte[currentChunkLength];
                Array.Copy(bytesArray, startIndex, chunk, 0, currentChunkLength);
                chunksList.Add(chunk);
            }

            return chunksList;
        }

        public static void CreateFileFromBytes(byte[] fileBytes) {
            if (FileToReceiveMetaData == null) {
                PrintMessage(ConsoleMessageType[1] + " Meta data is null!", ConsoleColor.Cyan);
                return;
            } else if (FileToReceiveMetaData.FileName == null) {
                PrintMessage(ConsoleMessageType[1] + "File name itself is null!", ConsoleColor.Cyan);

                return;
            }

            string fileName = Encoding.UTF8.GetString(FileToReceiveMetaData.FileName);
            float fileSize = BitConverter.ToSingle(FileToReceiveMetaData.FileSizeMB);

            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\Received Files\\" + fileName, fileBytes);

            PrintMessage(ConsoleMessageType[1] + $" Created file: {fileName} ({fileSize}MB).", ConsoleColor.Cyan);
        }

        public static void RenderFileTransferVisual() {
            int fileTotalChunksAmount = BitConverter.ToInt32(FileToReceiveMetaData.FileChunksAmount);
            float percentageAmount = fileTotalChunksAmount == 0 ? 0 : 
                MathF.Round(((float)FileToReceiveReceivedChunksAmount / (float)fileTotalChunksAmount) * 100, 2);

            string percentVisualMessage = ConsoleMessageType[1] + $" \rReceiving file... " + $"{percentageAmount:0.##}% completed.";

            if (percentageVisualConsoleCursorTopIndex == null) {
                percentageVisualConsoleCursorTopIndex = Console.CursorTop;
            }
            
            PrintMessage(percentVisualMessage.PadRight(Math.Max(0, Console.WindowWidth - 1)), ConsoleColor.Cyan, (0, (int)percentageVisualConsoleCursorTopIndex), true);
        }
    }

}