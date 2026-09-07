using MainClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using static MainClient.MainClient;

namespace ClientToServerHandler {
    public static class ClientToServerHandler {

        public static Dictionary<Guid, TaskCompletionSource<ClientRequestInfoMetaData>> PendingServerRequestsDictionary { get; private set; } = new();
        public static Dictionary<Guid, TaskCompletionSource<ServerRequestInfoMetaData>> PendingClientRequestsDictionary { get; private set; } = new();

        public static int RequiredClientPingDuration { get; private set; } = -1;

        public static TcpClient ServerTcp { get; private set; }

        public static bool IsConnectedToServer { get; set; } = false;

        private static Task? runningPingTimerTask;
        private static System.Timers.Timer timer;

        private static bool hasPingTimerElapsed = false;

        public static async Task<NetworkStream> ConnectToServer() {
            ServerTcp = new TcpClient();

            await ServerTcp.ConnectAsync(SERVER_IP, SERVER_PORT);

            NetworkStream clientToServerStream = ServerTcp.GetStream();

            IsConnectedToServer = true;

            return clientToServerStream;
        }

        public static async Task RegisterToServer() {
            PrintMessage("Registering to server.", ConsoleColor.Blue);

            NetworkStream stream = IsConnectedToServer ? ServerTcp.GetStream() : await ConnectToServer();

            // Create a server request to send to the server
            ClientRequestInfoMetaData clientServerRequest = new ClientRequestInfoMetaData(ClientId, ClientPort, 1, null, 
                null, null, false, null, null);

            EncodeClientRequestInfoMetaData(clientServerRequest, out byte[] dataBytes, out byte[] dataBytesLength);

            // Send the request
            await SendBytesToServerViaStream(dataBytes, dataBytesLength, stream);

            // Start the listener
            StartListener();

            if (runningPingTimerTask != null)
                return;

            // Cuz I don't wanna add server tokens or anything like that, just start the ping timer here despite the client not technically knowing
            // if he got accepted by the server (eventhough he will always be)
            runningPingTimerTask = StartClientPingHandler();
        }

        public static ServerRequestInfoMetaData? DeserializeServerRequestMetaData(byte[] serverRequestMetaDataBytes) {
            string serverRequestMetaDataString = Encoding.UTF8.GetString(serverRequestMetaDataBytes);
            return JsonSerializer.Deserialize<ServerRequestInfoMetaData>(serverRequestMetaDataString);
        }

        public static async Task SendBytesToServerViaStream(byte[] bytesToSend, byte[] bytesToSendLength, NetworkStream serverStream) {
            await WriteLock.WaitAsync();
            try {
                await serverStream.WriteAsync(bytesToSendLength);
                await serverStream.WriteAsync(bytesToSend);
            } finally {
                WriteLock.Release();
            }
        }

        public static async Task SendBytesToServerViaStream(byte[] bytesToSend, NetworkStream serverStream) {
            byte[] bytesToSendLength = BitConverter.GetBytes(bytesToSend.Length);

            await WriteLock.WaitAsync();

            try {
                await serverStream.WriteAsync(bytesToSendLength);
                await serverStream.WriteAsync(bytesToSend);
            } finally {
                WriteLock.Release();
            }
        }

        public static async Task StartClientPingHandler() {
            RequiredClientPingDuration = await GetRequiredPingFrequencyInSeconds();

            timer = new System.Timers.Timer((RequiredClientPingDuration * 0.5) * 1000); // Multiply by 0.9 to have room for delay 

            timer.Elapsed += Ping_Timer_Elapsed;

            timer.Start();

            while (true) {
                if (hasPingTimerElapsed) {
                    timer.Restart();

                    hasPingTimerElapsed = false;
                }
            }
        }

        public static void Restart(this System.Timers.Timer timer) {
            timer.Stop();
            timer.Start();
        }

        public static void StopPingTimer() {
            timer.Stop();

            PrintMessage("Stopped timer", ConsoleColor.Blue);
        }

        private static async void Ping_Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e) {
            NetworkStream stream = ServerTcp.GetStream();
            // Send a ping to the server
            var pingRequest = new ClientRequestInfoMetaData(ClientId, ClientPort, 7, null, null, null, false, null, null);

            await SendBytesToServerViaStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(pingRequest)), stream);

            hasPingTimerElapsed = true;
        }

        public static async Task<int> GetRequiredPingFrequencyInSeconds() {
            NetworkStream stream = ServerTcp.GetStream();

            Guid requestId = Guid.NewGuid();
            string requestIdString = JsonSerializer.Serialize(requestId);
            byte[] requestIdStringBytes = Encoding.UTF8.GetBytes(requestIdString);

            var request = new ClientRequestInfoMetaData(ClientId, ClientPort, null, 8, null, null, false, requestIdStringBytes, null);

            // Ask the server what the required ping frequency is
            await SendBytesToServerViaStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request)), stream);

            var tcs = new TaskCompletionSource<ServerRequestInfoMetaData>(TaskCreationOptions.RunContinuationsAsynchronously);

            PendingClientRequestsDictionary[requestId] = tcs;

            var serverResponds = await tcs.Task;

            string serverRespondsString = Encoding.UTF8.GetString(serverResponds.PacketData);

            return int.Parse(serverRespondsString);
        }
    }
}