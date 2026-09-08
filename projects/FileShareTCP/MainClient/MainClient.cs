using System.Net;
using System.Text;
using System.Text.Json;

using static ClientListener.ClientListener;
using static ClientFileHandler.ClientFileHandler;
using static ClientToServerHandler.ClientToServerHandler;
using System.Drawing;

namespace MainClient {
    public static class MainClient {

        public const string SERVER_DOMAIN_NAME = "DenzelinosPC";
        public const string SERVER_IP = "192.168.68.124";
        public const int SERVER_PORT = 7777;

        public static string ClientId { get; private set; }
        public static int ClientPort { get; private set; }
        public static int ClientPortLength { get; private set; } = 4;

        public static TaskCompletionSource<int> clientQuestionInputTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public static string? clientToSendToId;

        public static SemaphoreSlim WriteLock { get; private set; } = new SemaphoreSlim(1, 1);
        public static object CommandLineLock = new object();

        public static IPHostEntry? IPHostEntry { get; private set; }
        public static IPAddress? IPAddress { get; private set; }
        public static IPEndPoint? IPEndPoint { get; private set; }

        public static Task? ListenerTask { get; private set; } = null;

        public static bool isWaitingForQuestionInput = false;

        public static Random Random { get; private set; } = new Random(); // Initialize an publicly-accessible Random to avoid unnecesarry new intialization

        private static async Task Main(string[] args) {
            // Generate the necessary client data
            ClientId = GenerateClientId(7, 12);
            ClientPort = GenerateClientPort(ClientPortLength);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"ID: {ClientId}");
            Console.WriteLine($"PORT: {ClientPort}");
            Console.ResetColor();


            InitializeNetworking();

            if (IPHostEntry == null || IPAddress == null || IPEndPoint == null) {
                throw new Exception("ERROR: Unable to initialize networking properly, please try again later!");
            }

            await StartClientInputHandler();
        }

        private static void InitializeNetworking() {
            IPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress = IPHostEntry.AddressList[0];
            IPEndPoint = new IPEndPoint(IPAddress, ClientPort);
        }

        private static Task StartClientInputHandler() {

            while (true) {
                string? userInput = null;

                DrawCommandPrompt();

                userInput = Console.ReadLine();

                if (isWaitingForQuestionInput) {
                    bool hasInputedCorrectly;
                    hasInputedCorrectly = HandleQuestionInput(userInput);

                    while (!hasInputedCorrectly) {
                        hasInputedCorrectly = HandleQuestionInput(userInput);
                    }

                    isWaitingForQuestionInput = false;
                } else {
                    HandleCommandInput(userInput);
                }
            }
        }

        private static void DrawCommandPrompt() {
            lock (CommandLineLock) {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Please type a command to start! ");
                Console.ResetColor();
            }
        }

        public static void PrintMessage(string message, ConsoleColor messageColor, (int left, int top)? cursorPositionIndex = null, bool useWrite = false) {
            lock (CommandLineLock) {
                // Add extra logic for keeping the please type a command to start line at the bottom

                (int left, int top)? oldCursorPositionIndex = null;
                if (cursorPositionIndex != null) {
                    oldCursorPositionIndex = Console.GetCursorPosition();

                    // Set the cursor position to the specified position
                    Console.SetCursorPosition(cursorPositionIndex.Value.left, cursorPositionIndex.Value.top);
                }

                Console.ForegroundColor = messageColor;
                if (useWrite) Console.Write(message);
                else Console.WriteLine(message);
                Console.ResetColor();

                // Reset the cursor position if moved
                if (oldCursorPositionIndex != null) Console.SetCursorPosition(oldCursorPositionIndex.Value.left, oldCursorPositionIndex.Value.top);
            }
        }

        private static async void HandleCommandInput(string input) {
            if (input == UserCommands[0]) { // Connect server
                PrintMessage("Connecting to server...", ConsoleColor.Blue);

                await RegisterToServer();
            } else if (input == UserCommands[1]) { // Request file transfer
                await AskServerForFileTransferPermission();
            } else if (input == UserCommands[2]) { // Send file
                PrintMessage("Sending file...", ConsoleColor.Cyan);

                int allowedBuffer = await GetCurrentAllowedFileChunkBuffer();
                bool fileTransferResult = await SendFileToOtherClient(allowedBuffer);

                if (fileTransferResult) {
                    PrintMessage(ConsoleMessageType[1] + " File send complete", ConsoleColor.Cyan);
                } else {
                    PrintMessage(ConsoleMessageType[1] + " File send failed", ConsoleColor.Cyan);
                }
            }
        }

        private static bool HandleQuestionInput(string input) {
            if (int.TryParse(input, out int respondsNumber)) {
                // Use some sort of TCS
                clientQuestionInputTcs.SetResult(respondsNumber);

                return true;
            } else {
                PrintMessage("This responds is not valid, please fill in a number!", ConsoleColor.Blue);

                return false;
            }
        }

        public static string EncodeClientRequestInfoMetaData(ClientRequestInfoMetaData clientRequestInfoMetaData,
            out byte[] clientRequestInfoMetaDataBytes, out byte[] clientRequestInfoMetaDataBytesLength) {

            string clientServerRequestDataString = JsonSerializer.Serialize<ClientRequestInfoMetaData>(clientRequestInfoMetaData);

            clientRequestInfoMetaDataBytes = Encoding.UTF8.GetBytes(clientServerRequestDataString);
            clientRequestInfoMetaDataBytesLength = BitConverter.GetBytes(clientRequestInfoMetaDataBytes.Length);

            return clientServerRequestDataString;
        }

        private static string GenerateClientId(int minLength, int maxLength) {
            int idLength = Random.Next(minLength, maxLength);
            int randomNumberValue;

            string idString = "";
            char idLetter;

            for (int i = 0; i < idLength; i++) {
                randomNumberValue = Random.Next(0, 26);

                idLetter = Convert.ToChar(randomNumberValue + 65);

                idString += idLetter;
            }

            return idString;
        }

        private static int GenerateClientPort(int length = 4) {
            string generatedClientPort = "";
            for (int i = 0; i < length; i++) {
                generatedClientPort += Random.Next(1, 9).ToString();
            }

            return int.Parse(generatedClientPort);
        }

        public static void StopServerConnection() {
            StopPingTimer();

            IsConnectedToServer = false;
        }

        public static void StartListener() {
            if (ListenerTask == null) {
                PrintMessage("Starting listener...", ConsoleColor.Blue);

                ListenerTask = StartAndRunClientListener();
            } else {
                PrintMessage("Listener is already active!", ConsoleColor.Blue);
            }
        }

        public enum PacketType : byte { // Make everything operate via/with bytes instead of direct strings
            Question = 1,
            Message = 2,
            FileChunk = 3,
            FileChunkEnd = 4,
            FileMetaData = 5,
            ExtraDataQuestion = 6,
            Kick = 7
        }

        private static readonly string[] UserCommands = {
            "connect server",
            "request file transfer",
            "send file"
        };

        public static readonly string[] ConsoleMessageType = {
            "[Server]",
            "[File Transfer]"
        };
    }

    public record ClientRequestInfoMetaData(string Id, int Port, byte? PacketType, byte? PacketCommand, byte[]? PacketData, 
        byte[]? PacketDataLength, bool HasExtraData, byte[]? ClientRequestId, byte[]? ServerRequestId);

    public record ServerRequestInfoMetaData(byte? PacketType, byte[]? PacketData, byte[]? PacketDataLength, bool HasExtraData, byte[]? ClientRequestId, byte[]? ServerRequestId);
}

// To Do
// Bug fixes:

// Fix at the end:
// Make a build