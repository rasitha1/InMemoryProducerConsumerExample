using System.Threading;
using System.Threading.Tasks;

namespace WebProducerConsumerExample.HostedServices
{
    public interface IFileWorker
    {
        Task Process(WorkMessage message, CancellationToken cancellationToken);
    }
}