using AutoMapper;
using messageqServer.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace messageqServer.Services
{
    public class BackgroundPrinter : IHostedService, IDisposable
    {
        // private readonly ICreateService _createService;
        private readonly ILogger<BackgroundPrinter> logger;
        private readonly IMapper _mapper;
        private readonly IServiceScopeFactory scopeFactory;

        public BackgroundPrinter(ILogger<BackgroundPrinter> logger, IMapper mapper, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this._mapper = mapper;
            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .CreateLogger();

            try
            {
                Log.Information(messageTemplate: "Starting the Program");

                var factory = new ConnectionFactory() { HostName = "localhost" , Port = 5673 , VirtualHost = "vhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "qwebapp",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        try
                        {
                            Log.Information(messageTemplate: "Receiving the messages");
                            var body = ea.Body.ToArray();


                            String jsonified = Encoding.UTF8.GetString(body);
                            QueReceive queReceive = JsonConvert.DeserializeObject<QueReceive>(jsonified);

                            Console.WriteLine("Pure json: {0}", jsonified);
                            Console.WriteLine("Tag: {0}", queReceive.Tag);

                            string[] ret = queReceive.Tag.Split('.');

                            Console.WriteLine("Pais:  {0}", ret[0]);
                            Console.WriteLine("Regiao: {0}", ret[1]);
                            Console.WriteLine("Sensor: {0}", ret[2]);

                            if (queReceive.Valor == "")
                            {
                                CreateEventRequest createEventRequest = new CreateEventRequest() { Pais = ret[0], Regiao = ret[1], Sensor = ret[2], Status = "Failed", Valor = queReceive.Valor, Timestamp = queReceive.Timestamp };

                                using (var scope = scopeFactory.CreateScope())
                                {
                                    var res = scope.ServiceProvider.GetRequiredService<ICreateService>();
                                    res.Create(createEventRequest);
                                }
                            }
                            else 
                            {
                                CreateEventRequest createEventRequest = new CreateEventRequest() { Pais = ret[0], Regiao = ret[1], Sensor = ret[2], Status = "Succeed", Valor = queReceive.Valor, Timestamp = queReceive.Timestamp };

                                using (var scope = scopeFactory.CreateScope())
                                {
                                    var res = scope.ServiceProvider.GetRequiredService<ICreateService>();
                                    res.Create(createEventRequest);
                                }
                            }

                                

                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            // log ex
                            Log.Fatal(ex, messageTemplate: "Something went wrong with delivery, doing a Nack");
                            channel.BasicNack(ea.DeliveryTag, false, false);
                        }
                    };
                    channel.BasicConsume(queue: "qwebapp",
                                         autoAck: false,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                // log ex
                Console.WriteLine(ex);
                Log.Fatal(messageTemplate: "Something went wrong");
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Print stop");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            
        }
    }
}
