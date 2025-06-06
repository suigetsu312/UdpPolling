using System.Net.Sockets;
using System.Net;
using WinFormsApp1;
using System.Threading.Channels;
using System.Diagnostics;
using System.Linq.Expressions;

public class UDPSocketWrapper : ISocket
{
    private readonly UdpClient _client;

    public IPEndPoint? RemoteEndPoint { get; private set; }
    public SocketSourceType SocketSourceType { get; set; }

    public event EventHandler<SocketPacket>? OnDataReceived;
    public event EventHandler<SocketErrorArgs>? OnError;
    public event EventHandler<SocketSendArgs>? OnSend;

    public UDPSocketWrapper(IPEndPoint endPoint, SocketSourceType SocketSourceType)
    {
        RemoteEndPoint = endPoint;
        this.SocketSourceType = SocketSourceType;
        _client = new UdpClient(endPoint.Port);
        _client.Client.Blocking = false;
    }

    public Task StartAsync() => Task.CompletedTask;
    public Task StopAsync()
    {
        _client.Close();
        return Task.CompletedTask;
    }

    public Task SendAsync(SocketSendArgs sendArgs)
    {
        try
        {
            _client.Send(sendArgs.Bytes, sendArgs.Bytes.Length, sendArgs.Dist);
            OnSend?.Invoke(this, sendArgs);  // 成功寄送
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new SocketErrorArgs(ex, sendArgs.Bytes));
        }
        return Task.CompletedTask;
    }

    public bool TryReceive(out SocketPacket packet)
    {
        packet = default!;
        try
        {
            if (_client.Available > 0)
            {
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                byte[] bytes = _client.Receive(ref remote);
                RemoteEndPoint = remote;
                packet = new SocketPacket(bytes, DateTime.UtcNow, SocketSourceType, remote);

                OnDataReceived?.Invoke(this, packet);  // ⚠️ 主動發事件
                return true;
            }
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.WouldBlock) {
            Debug.WriteLine("?");
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new SocketErrorArgs(ex, []));
        }
        return false;
    }
}
