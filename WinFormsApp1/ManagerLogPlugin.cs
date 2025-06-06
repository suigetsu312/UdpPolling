using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    using Serilog;

    public class ManagerLogPlugin : ISocketManagerPlugin, IDisposable
    {
        private readonly ILogger _logger;
        private SocketManager? _manager;
        private SocketSourceType _sourceType;

        public ManagerLogPlugin(string sourceName, string logFolder, SocketSourceType sourceType)
        {
            _sourceType = sourceType;

            _logger = new LoggerConfiguration()
                .Enrich.WithProperty("SourceType", sourceType) // 加入 log 事件的屬性
                .MinimumLevel.Debug()
                .WriteTo.Map(
                    keyPropertyName: "SourceType",
                    defaultKey: "Unknown",
                    configure: (key, wt) => wt.Async(a => a.File(
                        path: $"{logFolder}/{key}-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        shared: true)))
                .CreateLogger();
        }

        public void Attach(SocketManager manager)
        {
            _manager = manager;
            manager.OnPacketProcessed += Manager_OnPacketProcessed;
        }

        public void Detach()
        {
            if (_manager != null)
            {
                _manager.OnPacketProcessed -= Manager_OnPacketProcessed;
                _manager = null;
            }
        }

        private void Manager_OnPacketProcessed(object? sender, SocketPacket packet)
        {
            if(packet.SourceType == _sourceType)
            {
                _logger
                .ForContext("SourceType", packet.SourceType)
                .Information("[{Time}] {Source} -> {Data}",
                DateTime.Now.ToString("HH:mm:ss.fff"),
                packet.SRC,
                Encoding.UTF8.GetString(packet.Bytes));
            }
            
        }

        public void Dispose()
        {
            Detach();
            (_logger as IDisposable)?.Dispose();
        }
    }
}
