﻿using Microsoft.Extensions.DependencyInjection;

namespace SoraBot.Services.Waifu
{
    public static class WaifuServiceDependencyInjection
    {
        public static IServiceCollection AddWaifuServices(this IServiceCollection services)
        {
            services.AddScoped<IWaifuService, WaifuService>();
            
            return services;
        }        
    }
}