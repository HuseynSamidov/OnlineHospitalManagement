using Application.Abstracts.Services;
using Application.DTOs.EmailDTOs;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services;

public class EmailPublisher : IEmailPublisher
{
    private readonly ConnectionFactory _factory;

    public EmailPublisher(IConfiguration configuration)
    {
        _factory = new ConnectionFactory
        {
            Uri = new Uri(configuration.GetConnectionString("RabbitMq"))
        };
    }

    public Task PublishAsync(EmailMessageDto message)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("email-queue", durable: true, exclusive: false, autoDelete: false);

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        channel.BasicPublish(exchange: "",
                             routingKey: "email-queue",
                             basicProperties: null,
                             body: body);

        return Task.CompletedTask;
    }
}
