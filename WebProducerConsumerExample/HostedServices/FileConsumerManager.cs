using System;
using System.Collections.Concurrent;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebProducerConsumerExample.HostedServices
{
    public class FileConsumerManager : IFileConsumerManager
    {
        private readonly BlockingCollection<ControlMessage> _controlQueue;
        private readonly BlockingCollection<WorkMessage> _workQueue;
        private readonly IFileWorkerFactory _workerFactory;
        private readonly FileConsumerManagerOptions _options;
        private readonly ILogger<FileConsumerManager> _logger;
        private readonly ConcurrentQueue<ConsumerTask> _workers = new ConcurrentQueue<ConsumerTask>();
        private int _nextId = 0;

        private class ConsumerTask
        {
            public ConsumerTask(string id, Task task, CancellationTokenSource cancellationTokenSource)
            {
                Id = id;
                Task = task;
                CancellationTokenSource = cancellationTokenSource;
            }
            public Task Task { get; }
            public CancellationTokenSource CancellationTokenSource { get; }
            public string Id { get; }
        }

        public FileConsumerManager(BlockingCollection<ControlMessage> controlQueue, 
            BlockingCollection<WorkMessage> workQueue, 
            IFileWorkerFactory workerFactory,
            IOptions<FileConsumerManagerOptions> options,
            ILogger<FileConsumerManager> logger)
        {
            _controlQueue = controlQueue;
            _workQueue = workQueue;
            _workerFactory = workerFactory;
            _options = options.Value;
            _logger = logger;
        }
        private Task _task;
        public async Task EnsureConsumersRunning(CancellationToken stoppingToken)
        {
            if (_task != null)
            {
                return;
            }

            _task = Task.Factory.StartNew(async state =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = _controlQueue.Take(stoppingToken);
                        _logger.LogInformation("Got control message: {message}", message);
                        if (message.ConsumerCount != _workers.Count)
                        {
                            await AdjustConsumers(message.ConsumerCount, stoppingToken);
                        }

                        if (message.LogStats)
                        {
                            _logger.LogInformation("Worker Count:{WorkerCount}, QueueSize:{QueueSize}", _workers.Count, _workQueue.Count);
                        }
                    }
                    catch (OperationCanceledException )
                    {
                        _logger.LogInformation("Operation cancelled. Stopping main worker.");
                        break;
                    }
                }

            }, stoppingToken, TaskCreationOptions.LongRunning);

            if (_options.StartConsumersAtStartup)
            {
                _controlQueue.Add(new ControlMessage { ConsumerCount = _options.DefaultConsumerCount }, stoppingToken);
            }

            await Task.CompletedTask;
        }

        private async Task AdjustConsumers(int count, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _logger.LogInformation("Adjusting consumers: Current:{CurrentCount}, New:{NewCount}", _workers.Count, count);
                while (_workers.Count < count)
                {
                    var worker = StartNewWorker(cancellationToken);
                    _logger.LogInformation("Adding new worker to worker pool: {ID}", worker.Id);
                    _workers.Enqueue(worker);
                }

                while (_workers.Count > count)
                {
                    if (_workers.TryDequeue(out var toStop))
                    {
                        toStop.CancellationTokenSource.Cancel();
                    }

                }

            }, cancellationToken);
        }

        private  ConsumerTask StartNewWorker(CancellationToken cancellationToken)
        {
            // to stop all consumers and the runner logic, cancel this
            var tokenSource = new CancellationTokenSource();
            var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, tokenSource.Token);

            var id = Interlocked.Increment(ref _nextId);

            var task = Task.Factory.StartNew(async state =>
            {
                var worker = _workerFactory.GetWorker(id.ToString());
                while (true)
                {
                    try
                    {
                        // this is a blocking operation
                        var message = _workQueue.Take(linked.Token);
                        await worker.Process(message, tokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("Operation cancelled. Stopping worker: {ID}", id);
                        break;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Error in file worker: {ErrorMessage}", e.Message);
                    }

                }

            }, linked, TaskCreationOptions.LongRunning);

            return new ConsumerTask(id.ToString(), task, linked);
        }
    }
}