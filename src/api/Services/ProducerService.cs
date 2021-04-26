using Bogus;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StreamProcessor.Services
{
    public class ProducerService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9093"
            };
            var producer = new ProducerBuilder<string, string>(config).Build();
            
            await producer.ProduceAsync("test", new Message<string, string>()
            {
                Key = "112",
                Value = "Holalala"

            });
            Console.WriteLine("Publicado");
        }

    }
}
