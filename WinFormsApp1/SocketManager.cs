using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace WinFormsApp1
{

    public class SocketManager : IDisposable
    {
        private readonly ConcurrentDictionary<string, ISocket> _sockets = new();
        private readonly Channel<SocketPacket> _channel = Channel.CreateUnbounded<SocketPacket>();
        private CancellationTokenSource? _cts;
        private Task? _processingTask;
        private Task? _receivingTask;

        public event EventHandler<SocketPacket>? OnPacketProcessed;

        // 註冊新的 Socket 並訂閱它的資料事件
        public bool RegisterSocket(string name, ISocket socket)
        {
            if (_sockets.TryAdd(name, socket))
            {
                return true;
            }
            return false;
        }

        // 啟動管理器
        public void Start()
        {
            if (_cts != null) throw new InvalidOperationException("Already started");

            _cts = new CancellationTokenSource();
            _processingTask = Task.Run(() => ProcessingLoop(_cts.Token));
            _receivingTask = Task.Run(() => ReceiveLoop(_cts.Token));
        }

        // 停止管理器，並停止所有 Socket
        public async Task StopAsync()
        {
            if (_cts == null) return;

            _cts.Cancel();

            if (_processingTask != null)
                await _processingTask.ConfigureAwait(false);

            foreach (var socket in _sockets.Values)
            {
                await socket.StopAsync().ConfigureAwait(false);
            }

            _sockets.Clear();
            _cts.Dispose();
            _cts = null;
        }

        private async Task ReceiveLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                foreach (var socket in _sockets.Values)
                {
                    if (socket.TryReceive(out var packet))
                    {
                        
                        _channel.Writer.TryWrite(packet);
                        Debug.WriteLine("socket manager ReceiveLoop : " + Thread.CurrentThread.ManagedThreadId);

                    }
                }
                await Task.Delay(10, ct); // 控制頻率，避免太密集
            }
        }


        // 處理 Channel 中的封包
        private async Task ProcessingLoop(CancellationToken ct)
        {
            var reader = _channel.Reader;

            while (await reader.WaitToReadAsync(ct))
            {
                while (reader.TryRead(out var packet))
                {
                    try
                    {
                        Debug.WriteLine("socket manager ProcessingLoop : " + Thread.CurrentThread.ManagedThreadId);
                        OnPacketProcessed?.Invoke(this, packet);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[SocketManager] Packet processing error: {ex}");
                    }
                }
            }
        }

        // 動態加入新 Socket
        public async Task AddSocketAsync(string name, ISocket socket)
        {
            if (RegisterSocket(name, socket))
            {
                await socket.StartAsync().ConfigureAwait(false);
            }
        }

        // 動態移除 Socket
        public async Task RemoveSocketAsync(string name)
        {
            if (_sockets.TryRemove(name, out var socket))
            {
                await socket.StopAsync().ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            StopAsync().GetAwaiter().GetResult();
        }
    }
}
