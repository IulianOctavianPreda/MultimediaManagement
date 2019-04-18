using Microsoft.Extensions.DependencyInjection;
using MultimediaManagement.UoW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaManagement.Services
{
    public static class ServiceRegistration
    {
        public static void ServiceRegistrator(IServiceCollection services)
        {
            if (services != null)
            {  
                services.AddScoped<IUnitOfWork, UnitOfWork>();
               //services.AddScoped<ITblCourseRepository, TblCourseRepository>();

            }

        }
    }
}
