using ClickMe.Application.Services.Counters;
using Microsoft.Extensions.DependencyInjection;

namespace ClickMe.Application;

public static class DependencyInjection {
    public static IServiceCollection AddApplication(this IServiceCollection services) {
        services.AddScoped<ICounterService, CounterService>();
        
        return services;
    }
}