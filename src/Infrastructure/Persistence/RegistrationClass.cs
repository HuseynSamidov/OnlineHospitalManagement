using Application.Abstracts.Repositories;
using Application.Abstracts.Services;
using Application.Shared.RabbitMQ;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using Persistence.Services;

namespace Persistence;

public static class RegistrationClass
{
    public static void RegisterService(this IServiceCollection services)
    {
        #region Repo
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IQueueTicketRepository, QueueTicketRepository>();
        #endregion

        #region Service
        services.AddScoped<IUserService,UserService>();
        services.AddScoped<IQueueTicketService,QueueTicketService>();
        services.AddScoped<IDoctorService,DoctorService>();
        services.AddScoped<FileUploadService>();
        //services.AddSingleton<EmailPublisher>();
        services.AddScoped<IAppEmailService,EmailService>();
        //services.AddHostedService<EmailConsumer>();
        //services.AddHostedService<EmailConsumerService>();
        #endregion
    }
}
