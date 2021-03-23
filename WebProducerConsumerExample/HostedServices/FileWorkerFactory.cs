using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebProducerConsumerExample.HostedServices
{
    class FileWorkerFactory : IFileWorkerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ObjectFactory _factory;
        public FileWorkerFactory(IServiceProvider serviceProvider, IHttpClientFactory clientFactory)
        {
            _serviceProvider = serviceProvider;
            _clientFactory = clientFactory;
            _factory = ActivatorUtilities.CreateFactory(typeof(FileWorker), new Type[] { typeof(string), typeof(HttpClient) });
        }
        public IFileWorker GetWorker(string id)
        {
            var client = _clientFactory.CreateClient(id);
            return (IFileWorker)_factory(_serviceProvider, new object[] { id, client });
        }
    }
}