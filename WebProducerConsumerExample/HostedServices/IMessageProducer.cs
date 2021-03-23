using System.Threading.Tasks;

namespace WebProducerConsumerExample.HostedServices
{
    public interface IMessageProducer
    {
        Task SendControlMessage(ControlMessage message);
        Task SendWorkMessage(WorkMessage message);
    }
}