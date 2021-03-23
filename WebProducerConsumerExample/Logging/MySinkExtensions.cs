using System;
using Serilog;
using Serilog.Configuration;

namespace WebProducerConsumerExample.Logging
{
    public static class MySinkExtensions
    {
        public static LoggerConfiguration MySink(
            this LoggerSinkConfiguration loggerConfiguration,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new MySink(formatProvider));
        }
    }
}