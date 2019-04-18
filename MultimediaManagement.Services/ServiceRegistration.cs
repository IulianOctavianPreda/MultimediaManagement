using Microsoft.Extensions.DependencyInjection;
using MultimediaManagement.Dao.Models;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaManagement.Services
{
    public static class ServiceRegistration
    {
        public static void ServiceRegistrator(this IServiceCollection services)
        {
            if (services != null)
            {
                var connection = @"Server=DESKTOP-UONO7AU\SQLEXPRESS;Database=WADMultimediaManagement;Trusted_Connection=True;ConnectRetryCount=0";
                services.AddDbContext<MultimediaManagementContext>(options => options.UseSqlServer(connection));
                //services.AddScoped<>;

            }

        }
    }
}
