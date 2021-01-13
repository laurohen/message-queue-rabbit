using AutoMapper;
using messageqServer.Models;
using messageqServer.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messageqServer.Service
{
    public class GetQ 
    {
        private readonly ICreateService _createService;
        private readonly IMapper _mapper;

        //public GetQ()
        //{
        //}

        public GetQ(
            ICreateService createService,
            IMapper mapper)
        {
            this._createService = createService;
            _mapper = mapper;
        }
        public void ReceiveMsg()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .CreateLogger();

            try
            {
                Log.Information(messageTemplate: "Starting the Program");

                var factory = new ConnectionFactory() { HostName = "localhost" };
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
                            //var message = Encoding.UTF8.GetString(body);
                            //var receiveBytes = JsonConvert.SerializeObject(message);
                            
                            String jsonified = Encoding.UTF8.GetString(body);
                            QueReceive queReceive = JsonConvert.DeserializeObject<QueReceive>(jsonified);

                            Console.WriteLine("Pure json: {0}", jsonified);
                            Console.WriteLine("Tag: {0}", queReceive.Tag);

                            string[] ret = queReceive.Tag.Split('.');

                            Console.WriteLine("Pais:  {0}", ret[0]);
                            Console.WriteLine("Regiao: {0}", ret[1]);
                            Console.WriteLine("Sensor: {0}", ret[2]);

                            CreateEventRequest createEventRequest = new CreateEventRequest() { Pais = ret[0], Regiao = ret[1], Sensor = ret[2], Status = "ok", Valor = queReceive.Valor, Timestamp = queReceive.Timestamp };


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
        }
    }
}
