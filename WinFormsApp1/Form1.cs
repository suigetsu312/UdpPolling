using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        SocketManager manager;
        private CancellationTokenSource _sendCts = new();
        LogPlugin logPlugin;
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // 建立 SocketManager
            manager = new SocketManager();
            var loggerEvent = (string s) =>
            {
            };
            logPlugin = new LogPlugin(loggerEvent);
            // 註冊事件，收到封包時輸出內容
            manager.OnPacketProcessed += (s, packet) =>
            {
                string text = Encoding.UTF8.GetString(packet.Bytes);

            };

            // 建立兩個 UDP Socket（模擬兩個來源）
            var udp1 = new UDPSocketWrapper(new IPEndPoint(IPAddress.Loopback, 9000), SocketSourceType.GPS);
            var udp2 = new UDPSocketWrapper(new IPEndPoint(IPAddress.Loopback, 9001), SocketSourceType.IMU);
            var gpsLogger = new ManagerLogPlugin("GPS", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log"), SocketSourceType.GPS);
            var imuLogger = new ManagerLogPlugin("IMU", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log"), SocketSourceType.IMU);

            logPlugin.Attach(udp2);
            logPlugin.Attach(udp1);
            manager.AddPlugin(gpsLogger);
            manager.AddPlugin(imuLogger);


            // 加入並啟動 Socket
            await manager.AddSocketAsync("GPS", udp1);
            await manager.AddSocketAsync("IMU", udp2);

            // 啟動 SocketManager 的資料處理 Loop
            manager.Start();

        }

        private void StartSendingPackets()
        {
            _sendCts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                var udpClient = new UdpClient();
                var targetEP = new IPEndPoint(IPAddress.Loopback, 9000); // UDP1 監聽的 port

                while (!_sendCts.Token.IsCancellationRequested)
                {
                    var data = Encoding.UTF8.GetBytes($"GPS {DateTime.Now}");
                    await udpClient.SendAsync(data, data.Length, targetEP);
                    await Task.Delay(1000); // 1秒一次
                }
            }, _sendCts.Token);

            Task.Run(async () =>
            {
                var udpClient = new UdpClient();
                var targetEP = new IPEndPoint(IPAddress.Loopback, 9001); // UDP2 監聽的 port

                while (!_sendCts.Token.IsCancellationRequested)
                {
                    var data = Encoding.UTF8.GetBytes($"IMU {DateTime.Now}");
                    await udpClient.SendAsync(data, data.Length, targetEP);
                    await Task.Delay(1200); // 1.2秒一次
                }
            }, _sendCts.Token);
        }

        private void StopSendingPackets()
        {
            _sendCts.Cancel();
            _sendCts.Dispose();
        }
        private void button_send_Click(object sender, EventArgs e)
        {
            StartSendingPackets();
            button_send.Enabled = false;
            button_endsend.Enabled = true;
        }

        private void button_endsend_Click(object sender, EventArgs e)
        {
            StopSendingPackets();
            button_send.Enabled = true;
            button_endsend.Enabled = false;

        }
    }
}
