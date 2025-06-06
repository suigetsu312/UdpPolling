using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public interface ISocket
    {
        SocketSourceType SocketSourceType { get; }
        IPEndPoint? RemoteEndPoint { get; }
        Task StartAsync();
        Task StopAsync();
        Task SendAsync(SocketSendArgs sendArgs);
        public bool TryReceive(out SocketPacket packet);

        event EventHandler<SocketPacket>? OnDataReceived;
        event EventHandler<SocketErrorArgs>? OnError;
        event EventHandler<SocketSendArgs>? OnSend;

    }
}
