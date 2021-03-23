using System;
using Serilog.Core;
using Serilog.Events;

namespace WebProducerConsumerExample.Logging
{
    public class MySink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;


        public MySink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            LogQueue.Queue.Add(new LogMessage((int)logEvent.Level, message));
        }
    }
}