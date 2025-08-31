using Application.Abstracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;

namespace Persistence;

public static class RegistrationClass
{
    public static void RegisterService(this IServiceCollection services)
    {
        #region Repo
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IMedicalServiceRepository, MedicalServiceRepository>();
            services.AddScoped<IQueueTicketRepository, QueueTicketRepository>();
        #endregion

        #region Service

        #endregion


    }

}
