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
            // �إ� SocketManager
            manager = new SocketManager();
            var loggerEvent = (string s) =>
            {
                Debug.WriteLine(s);
            };
            logPlugin = new LogPlugin(loggerEvent);
            // ���U�ƥ�A����ʥ]�ɿ�X���e
            manager.OnPacketProcessed += (s, packet) =>
            {
                string text = Encoding.UTF8.GetString(packet.Bytes);
                Console.WriteLine($"[{packet.SRC}] {packet.ReceivedTime:HH:mm:ss.fff} -> {text}");
            };

            // �إߨ�� UDP Socket�]������Өӷ��^
            var udp1 = new UDPSocketWrapper(new IPEndPoint(IPAddress.Loopback, 9000), SocketSourceType.GPS);
            var udp2 = new UDPSocketWrapper(new IPEndPoint(IPAddress.Loopback, 9001), SocketSourceType.IMU);
            logPlugin.Attach(udp2);
            logPlugin.Attach(udp1);
            udp1.OnDataReceived += (s, packet) =>
            {
                Debug.WriteLine("udp1 user define on receive " + Thread.CurrentThread.ManagedThreadId);

                // �������ާ@ UI �n�� Invoke
                if (richTextBox_socket1.InvokeRequired)
                {
                    richTextBox_socket1.Invoke(new Action(() =>
                    {
                        string text = Encoding.UTF8.GetString(packet.Bytes);
                        richTextBox_socket1.Text = ($"[{packet.SRC}] {packet.ReceivedTime:HH:mm:ss.fff} -> {text}");
                    }));
                }
                else
                {
                    string text = Encoding.UTF8.GetString(packet.Bytes);
                    richTextBox_socket1.Text = ($"[{packet.SRC}] {packet.ReceivedTime:HH:mm:ss.fff} -> {text}");
                }
            };

            udp2.OnDataReceived += (s, packet) =>
            {
                Debug.WriteLine("udp2 user define on receive " + Thread.CurrentThread.ManagedThreadId);

                if (richTextBox_socket2.InvokeRequired)
                {
                    richTextBox_socket2.Invoke(new Action(() =>
                    {
                        string text = Encoding.UTF8.GetString(packet.Bytes);
                        richTextBox_socket2.Text = ($"[{packet.SRC}] {packet.ReceivedTime:HH:mm:ss.fff} -> {text}");
                    }));
                }
                else
                {
                    string text = Encoding.UTF8.GetString(packet.Bytes);
                    richTextBox_socket2.Text = ($"[{packet.SRC}] {packet.ReceivedTime:HH:mm:ss.fff} -> {text}");
                }
            };

            // �[�J�ñҰ� Socket
            await manager.AddSocketAsync("UDP1", udp1);
            await manager.AddSocketAsync("UDP2", udp2);

            // �Ұ� SocketManager ����ƳB�z Loop
            manager.Start();

        }

        private void StartSendingPackets()
        {
            _sendCts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                var udpClient = new UdpClient();
                var targetEP = new IPEndPoint(IPAddress.Loopback, 9000); // UDP1 ��ť�� port

                while (!_sendCts.Token.IsCancellationRequested)
                {
                    var data = Encoding.UTF8.GetBytes($"Hello UDP1 {DateTime.Now:HH:mm:ss.fff}");
                    await udpClient.SendAsync(data, data.Length, targetEP);
                    await Task.Delay(1000); // 1��@��
                }
            }, _sendCts.Token);

            Task.Run(async () =>
            {
                var udpClient = new UdpClient();
                var targetEP = new IPEndPoint(IPAddress.Loopback, 9001); // UDP2 ��ť�� port

                while (!_sendCts.Token.IsCancellationRequested)
                {
                    var data = Encoding.UTF8.GetBytes($"Hello UDP2 {DateTime.Now:HH:mm:ss.fff}");
                    await udpClient.SendAsync(data, data.Length, targetEP);
                    await Task.Delay(1200); // 1.2��@��
                }
            }, _sendCts.Token);
        }

        private void StopSendingPackets()
        {
            _sendCts.Cancel();
            _sendCts.Dispose();
        }


        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (manager != null)
            {
                await manager.StopAsync();
                manager.Dispose();
            }
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
