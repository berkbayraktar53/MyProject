﻿using Core.Utilities.IoC;
using Core.CrossCuttingConcerns.Caching;
using Microsoft.Extensions.DependencyInjection;
using Core.CrossCuttingConcerns.Caching.Microsoft;

namespace Core.DependencyResolvers
{
    public class CoreModule : ICoreModule
    {
        public void Load(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheManager, MemoryCacheManager>();
        }
    }
}