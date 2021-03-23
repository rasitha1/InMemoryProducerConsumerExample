using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WebProducerConsumerExample.HostedServices
{
    public class FileWorker : IFileWorker
    {
        private readonly string _id;
        private readonly HttpClient _client;
        private readonly ILogger _logger;

        public FileWorker(string id, HttpClient client, ILoggerFactory loggerFactory)
        {
            _id = id;
            _client = client;
            _logger = loggerFactory.CreateLogger($"{typeof(FileWorker).FullName}:{id}");
        }
        public async Task Process(WorkMessage message, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            var response = await _client.GetAsync(message.Url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            sw.Stop();
            var total = (DateTime.UtcNow - message.Started).TotalMilliseconds;
            _logger.LogInformation("Content Size: {ContentLength}. Time Taken: {TimeTaken}. Worker:{WorkerId} Total:{TotalTime}", content.Length, sw.ElapsedMilliseconds, _id, total);
            if (message.LastInBatch)
            {
                _logger.LogInformation("Batch - Done");
            }
        }
    }
}