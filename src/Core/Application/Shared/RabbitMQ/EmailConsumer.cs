using Application.Abstracts.Services;
using Application.DTOs.EmailDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Application.Shared.RabbitMQ;

public class EmailConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConnectionFactory _factory;

    public EmailConsumer(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _factory = new ConnectionFactory
        {
            Uri = new Uri(configuration.GetConnectionString("RabbitMq"))
        };
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare("email-queue", durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (ch, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var message = JsonConvert.DeserializeObject<EmailMessageDto>(json);

            if (message != null)
            {
                using var scope = _serviceProvider.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IAppEmailService>();

                await emailService.SendEmailAsync(message.To, message.Subject, message.Body);
            }
        };

        channel.BasicConsume("email-queue", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
