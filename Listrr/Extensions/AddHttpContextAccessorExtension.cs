using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Listrr.Extensions
{
    public static class AddHttpContextAccessorExtension
    {

        public static IServiceCollection AddHttpContextAccessor(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }

    }
}