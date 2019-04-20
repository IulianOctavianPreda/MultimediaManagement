using Microsoft.Extensions.DependencyInjection;
using MultimediaManagement.Repository;

namespace MultimediaManagement.Services
{
    public static class ServiceRegistration
    {
        public static void ServiceRegistrator(IServiceCollection services)
        {
            if (services != null)
            {  
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IPlaceholderRepository, PlaceholderRepository>();
                services.AddScoped<ICollectionRepository, CollectionRepository>();
                services.AddScoped<IEntityFileRepository, EntityFileRepository>();
            }
        }
    }
}
