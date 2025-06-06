using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class LogPlugin : ISocketPlugin
    {
        private readonly Action<string> _logAction;

        public LogPlugin(Action<string>? logAction = null)
        {
            _logAction = logAction ?? Console.WriteLine;
        }

        public void Attach(ISocket socket)
        {
            socket.OnDataReceived += (s, packet) =>
            {
                string log = $"logger [{DateTime.Now:HH:mm:ss.fff}] [{socket.SocketSourceType}] {packet.SRC} -> {BitConverter.ToString(packet.Bytes)}";
                _logAction(log);
            };

            socket.OnError += (s, err) =>
            {
                string log = $"logger [Error] [{socket.SocketSourceType}] {err.exception.Message}";
                _logAction(log);
            };

            socket.OnSend += (s, send) =>
            {
                string log = $"logger [Sent] [{socket.SocketSourceType}] To {send.Dist} : {BitConverter.ToString(send.Bytes)}";
                _logAction(log);
            };
        }
    }
}
