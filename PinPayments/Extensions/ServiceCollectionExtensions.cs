using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace PinPayments.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPinPayments(
            this IServiceCollection services, 
            PinPaymentsOptions configureOptions
        )
        {
            configureOptions.CheckArgumentNull(nameof(configureOptions));

            services.TryAddSingleton(Options.Create(configureOptions));
            services.TryAddTransient<IPinService, PinService>();
        }

        public static void AddPinPayments(
            this IServiceCollection services, 
            Action<PinPaymentsOptions> configuration)
        {
            configuration.CheckArgumentNull(nameof(configuration));

            var configureOptions = new PinPaymentsOptions();

            configuration(configureOptions);

            AddPinPayments(services, configureOptions);
        }

        public static void AddPinPayments(
            this IServiceCollection services, 
            IConfigurationRoot configuration
        )
        {
            configuration.CheckArgumentNull(nameof(configuration));

            services.Configure<PinPaymentsOptions>(
                configuration.GetSection("PinPayments")
            );
            
            services.TryAddTransient<IPinService, PinService>();
        }
    }
}