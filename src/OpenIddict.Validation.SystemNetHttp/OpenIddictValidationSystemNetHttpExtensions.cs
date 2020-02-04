﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using OpenIddict.Validation;
using OpenIddict.Validation.SystemNetHttp;
using static OpenIddict.Validation.SystemNetHttp.OpenIddictValidationSystemNetHttpHandlerFilters;
using static OpenIddict.Validation.SystemNetHttp.OpenIddictValidationSystemNetHttpHandlers;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes extensions allowing to register the OpenIddict validation/System.Net.Http integration services.
    /// </summary>
    public static class OpenIddictValidationSystemNetHttpExtensions
    {
        /// <summary>
        /// Registers the OpenIddict validation/System.Net.Http integration services in the DI container.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictValidationBuilder"/>.</returns>
        public static OpenIddictValidationSystemNetHttpBuilder UseSystemNetHttp([NotNull] this OpenIddictValidationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddHttpClient();

            // Register the built-in validation event handlers used by the OpenIddict System.Net.Http components.
            // Note: the order used here is not important, as the actual order is set in the options.
            builder.Services.TryAdd(DefaultHandlers.Select(descriptor => descriptor.ServiceDescriptor));

            // Register the built-in filters used by the default OpenIddict System.Net.Http event handlers.
            builder.Services.TryAddSingleton<RequireHttpMetadataAddress>();

            // Note: TryAddEnumerable() is used here to ensure the initializers are registered only once.
            builder.Services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Singleton<IConfigureOptions<OpenIddictValidationOptions>, OpenIddictValidationSystemNetHttpConfiguration>(),
                ServiceDescriptor.Singleton<IConfigureOptions<HttpClientFactoryOptions>, OpenIddictValidationSystemNetHttpConfiguration>()
            });

            return new OpenIddictValidationSystemNetHttpBuilder(builder.Services);
        }

        /// <summary>
        /// Registers the OpenIddict validation/System.Net.Http integration services in the DI container.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <param name="configuration">The configuration delegate used to configure the validation services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictBuilder"/>.</returns>
        public static OpenIddictValidationBuilder UseSystemNetHttp(
            [NotNull] this OpenIddictValidationBuilder builder,
            [NotNull] Action<OpenIddictValidationSystemNetHttpBuilder> configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            configuration(builder.UseSystemNetHttp());

            return builder;
        }
    }
}
