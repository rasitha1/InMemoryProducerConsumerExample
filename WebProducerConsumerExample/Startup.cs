using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebProducerConsumerExample.HostedServices;
using WebProducerConsumerExample.Logging;

namespace WebProducerConsumerExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddTransient<IFileConsumerManager, FileConsumerManager>();
            services.AddSingleton<BlockingCollection<ControlMessage>>();
            services.AddSingleton<BlockingCollection<WorkMessage>>();
            services.AddSingleton<IFileWorkerFactory, FileWorkerFactory>();

            services.AddSingleton<MessagePump>();
            services.AddSingleton<Queue<LogMessage>>();

            services.AddTransient<IMessageProducer, BlockingQueueMessageProducer>();
            // can easily be another type
            //services.AddTransient<IMessageProducer, RabbitMQMessageProducer>();

            //services.AddTransient<IFileWorker, FileWorker>();

            for (int i = 0; i < 100; i++)
            {
                services.AddHttpClient($"{i}")
                    .ConfigurePrimaryHttpMessageHandler(provider =>
                    {
                        var handler = new HttpClientHandler
                        {
                            MaxConnectionsPerServer = 100
                        };

                        if (handler.SupportsAutomaticDecompression)
                        {
                            handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                        }
                        return handler;
                    });
            }

            services.AddSwaggerGen();

            services.AddSignalR();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "My API V1");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<LogHub>("/hubs/log");
            });
        }
    }
}
