namespace WebProducerConsumerExample.HostedServices
{
    public interface IFileWorkerFactory
    {
        IFileWorker GetWorker(string id);
    }
}