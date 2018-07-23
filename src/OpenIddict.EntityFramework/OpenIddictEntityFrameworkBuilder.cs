﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.ComponentModel;
using System.Data.Entity;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Core;
using OpenIddict.EntityFramework;
using OpenIddict.EntityFramework.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes the necessary methods required to configure the OpenIddict Entity Framework 6.x services.
    /// </summary>
    public class OpenIddictEntityFrameworkBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OpenIddictEntityFrameworkBuilder"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public OpenIddictEntityFrameworkBuilder([NotNull] IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        /// <summary>
        /// Gets the services collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceCollection Services { get; }

        /// <summary>
        /// Amends the default OpenIddict Entity Framework 6.x configuration.
        /// </summary>
        /// <param name="configuration">The delegate used to configure the OpenIddict options.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictEntityFrameworkBuilder"/>.</returns>
        public OpenIddictEntityFrameworkBuilder Configure([NotNull] Action<OpenIddictEntityFrameworkOptions> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Services.Configure(configuration);

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entities, derived
        /// from the default OpenIddict Entity Framework 6.x entities.
        /// </summary>
        /// <returns>The <see cref="OpenIddictEntityFrameworkBuilder"/>.</returns>
        public OpenIddictEntityFrameworkBuilder ReplaceDefaultEntities<TApplication, TAuthorization, TScope, TToken, TKey>()
            where TApplication : OpenIddictApplication<TKey, TAuthorization, TToken>
            where TAuthorization : OpenIddictAuthorization<TKey, TApplication, TToken>
            where TScope : OpenIddictScope<TKey>
            where TToken : OpenIddictToken<TKey, TApplication, TAuthorization>
            where TKey : IEquatable<TKey>
        {
            Services.Configure<OpenIddictCoreOptions>(options =>
            {
                options.DefaultApplicationType = typeof(TApplication);
                options.DefaultAuthorizationType = typeof(TAuthorization);
                options.DefaultScopeType = typeof(TScope);
                options.DefaultTokenType = typeof(TToken);
            });

            return this;
        }

        /// <summary>
        /// Configures the OpenIddict Entity Framework 6.x stores to use the specified database context type.
        /// </summary>
        /// <typeparam name="TContext">The type of the <see cref="DbContext"/> used by OpenIddict.</typeparam>
        /// <returns>The <see cref="OpenIddictEntityFrameworkBuilder"/>.</returns>
        public OpenIddictEntityFrameworkBuilder UseDbContext<TContext>()
            where TContext : DbContext
            => UseDbContext(typeof(TContext));

        /// <summary>
        /// Configures the OpenIddict Entity Framework 6.x stores to use the specified database context type.
        /// </summary>
        /// <param name="type">The type of the <see cref="DbContext"/> used by OpenIddict.</param>
        /// <returns>The <see cref="OpenIddictEntityFrameworkBuilder"/>.</returns>
        public OpenIddictEntityFrameworkBuilder UseDbContext([NotNull] Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(DbContext).IsAssignableFrom(type))
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            Services.TryAddScoped(type);

            return Configure(options => options.DbContextType = type);
        }
    }
}
