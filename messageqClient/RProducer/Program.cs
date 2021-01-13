using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace RProducer
{
    class Program
    {
        static void Main(string[] args)
        {

            ProduceMsg();
 
        }

        static void ProduceMsg()
        {
            string pais;
            string regiao = "";
            string sensor;
            string valor;

            string userInput;
            int caseSwitch;

            Console.Clear();
            Console.WriteLine("Escreva o país:  ");
            pais = Console.ReadLine();

            do
            {
                Console.Clear();
                Console.WriteLine("Ecolha a regiao:  ");
                Console.WriteLine("1 - Norte ");
                Console.WriteLine("2 - Nordeste ");
                Console.WriteLine("3 - Sul ");
                Console.WriteLine("4 - Sudeste ");
                Console.WriteLine("5 - Centro-Oeste ");
                userInput = Console.ReadLine();
                caseSwitch = Convert.ToInt32(userInput);
            } while (!(caseSwitch < 6));
            
            switch (caseSwitch)
            {
                case 1:
                    regiao = "norte";
                    break;
                case 2:
                    regiao = "nordeste";
                    break;
                case 3:
                    regiao = "sul";
                    break;
                case 4:
                    regiao = "sudeste";
                    break;
                case 5:
                    regiao = "centro-oeste";
                    break;
            }

            Console.Clear();
            Console.WriteLine("Escolhe o numero do sensor entre 0001 e 1000:  ");
            sensor = Console.ReadLine();

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.RollingFile(@"e:\log-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
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

                    String UUID = Guid.NewGuid().ToString();
                    while (true)
                    {
                        DateTime date = DateTime.Now;

                        Random randNum = new Random();

                        if((randNum.Next(1, 11))> 3)
                        {
                            Evento evento = new Evento() { Timestamp = date.ToString("dd/MM/yyyy HH:mm:ss"), Tag = $"{pais}.{regiao}.sensor{sensor}", Valor = $"{randNum.Next(1, 400)}" };

                            String jsonified = JsonConvert.SerializeObject(evento);
                            byte[] eventoBuffer = Encoding.UTF8.GetBytes(jsonified);


                            channel.BasicPublish(exchange: "",
                                                 routingKey: "qwebapp",
                                                 basicProperties: null,
                                                 body: eventoBuffer);
                            Console.WriteLine(" [x] Sent {0}", jsonified);

                            System.Threading.Thread.Sleep(1000);
                        }
                        else
                        {
                            Evento evento = new Evento() { Timestamp = date.ToString("dd/MM/yyyy HH:mm:ss"), Tag = $"{pais}.{regiao}.sensor{sensor}", Valor = "" };

                            String jsonified = JsonConvert.SerializeObject(evento);
                            byte[] eventoBuffer = Encoding.UTF8.GetBytes(jsonified);


                            channel.BasicPublish(exchange: "",
                                                 routingKey: "qwebapp",
                                                 basicProperties: null,
                                                 body: eventoBuffer);
                            Console.WriteLine(" [x] Sent {0}", jsonified);

                            System.Threading.Thread.Sleep(1000);
                        }

                        
                    }

                }
            }
            catch (Exception ex)
            {
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
