using ClickMe.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClickMe.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContextFactory<ClickMeDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        
        return services;
    }
}
