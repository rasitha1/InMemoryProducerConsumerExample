using System.Threading;
using System.Threading.Tasks;

namespace WebProducerConsumerExample.HostedServices
{
    public interface IFileConsumerManager
    {
        Task EnsureConsumersRunning(CancellationToken stoppingToken);
    }
}