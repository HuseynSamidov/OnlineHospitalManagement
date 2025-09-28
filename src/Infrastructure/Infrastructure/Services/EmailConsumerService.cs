using Application.Abstracts.Services;
using Application.DTOs.EmailDTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

//public class EmailConsumerService : BackgroundService
//{
//    private readonly ILogger<EmailConsumerService> _logger;
//    private readonly IConfiguration _configuration;
//    private readonly IAppEmailService _emailSender;
//    private readonly ConnectionFactory _factory;

//    public EmailConsumerService(
//        ILogger<EmailConsumerService> logger,
//        IConfiguration configuration,
//        IAppEmailService emailSender)
//    {
//        _logger = logger;
//        _configuration = configuration;
//        _emailSender = emailSender;
//        _factory = new ConnectionFactory
//        {
//            Uri = new Uri(configuration.GetConnectionString("RabbitMq")!)
//        };
//    }

//    protected override Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        var connection = _factory.CreateConnection();
//        var channel = connection.CreateModel();
//        var queue = _configuration["RabbitMQ:Queue"];
//        var exchange = _configuration["RabbitMQ:Exchange"];
//        var routingKey = _configuration["RabbitMQ:RoutingKey"];

//        channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true);
//        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
//        channel.QueueBind(queue, exchange, routingKey);

//        var consumer = new EventingBasicConsumer(channel);
//        consumer.Received += async (model, ea) =>
//        {
//            var body = ea.Body.ToArray();
//            var json = Encoding.UTF8.GetString(body);
//            var message = JsonConvert.DeserializeObject<EmailMessageDto>(json);

//            _emailSender.SendEmailAsync(message.To, message.Subject, message.Body);
//            _logger.LogInformation($"Email sent to {message.To}");
//        };

//        channel.BasicConsume(queue, autoAck: true, consumer: consumer);
//        return Task.CompletedTask;
//    }
//}
