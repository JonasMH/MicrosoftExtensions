﻿// <copyright file="AppMetricsServiceCollectionExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics;
using App.Metrics.Filters;
using App.Metrics.Formatters;
using App.Metrics.Infrastructure;
using App.Metrics.Internal.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
    // ReSharper restore CheckNamespace
{
    /// <summary>
    ///     Extension methods on <see cref="IServiceCollection" /> to register App Metrics services via an
    ///     <see cref="IMetricsBuilder" />.
    /// </summary>
    public static class AppMetricsServiceCollectionExtensions
    {
        public static IServiceCollection AddMetrics(this IServiceCollection services, Action<IMetricsBuilder> setupAction)
        {
            var builder = new MetricsBuilder();
            setupAction(builder);

            return AddMetrics(services, builder);
        }

        public static IServiceCollection AddMetrics(this IServiceCollection services, IMetricsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var metrics = builder.Build();

            AddCoreServices(services, metrics);

            return services;
        }

        public static IServiceCollection AddMetrics(this IServiceCollection services)
        {
            var builder = AppMetrics.CreateDefaultBuilder();

            return services.AddMetrics(builder);
        }

        private static void AddCoreServices(IServiceCollection services, IMetricsRoot metrics)
        {
            services.TryAddSingleton<IClock>(metrics.Clock);
            services.TryAddSingleton<IFilterMetrics>(metrics.Filter);
            services.TryAddSingleton<IMetricsOutputFormatter>(metrics.DefaultOutputMetricsFormatter);
            services.TryAddSingleton<MetricsFormatterCollection>(metrics.OutputMetricsFormatters);
            services.TryAddSingleton<IEnvOutputFormatter>(metrics.DefaultOutputEnvFormatter);
            services.TryAddSingleton<EnvFormatterCollection>(metrics.OutputEnvFormatters);
            services.TryAddSingleton<EnvironmentInfoProvider>(new EnvironmentInfoProvider());
            services.TryAddSingleton<IMetrics>(metrics);
            services.TryAddSingleton<MetricsOptions>(metrics.Options);
            services.TryAddSingleton<AppMetricsMarkerService, AppMetricsMarkerService>();
        }
    }
}