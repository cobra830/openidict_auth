﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.ComponentModel;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Owin;
using OpenIddict.Server.Owin;
using Owin;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes the necessary methods required to configure
    /// the OpenIddict server OWIN/Katana integration.
    /// </summary>
    public class OpenIddictServerOwinBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OpenIddictServerOwinBuilder"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public OpenIddictServerOwinBuilder([NotNull] IServiceCollection services)
            => Services = services ?? throw new ArgumentNullException(nameof(services));

        /// <summary>
        /// Gets the services collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceCollection Services { get; }

        /// <summary>
        /// Amends the default OpenIddict server OWIN/Katana configuration.
        /// </summary>
        /// <param name="configuration">The delegate used to configure the OpenIddict options.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictServerOwinBuilder"/>.</returns>
        public OpenIddictServerOwinBuilder Configure([NotNull] Action<OpenIddictServerOwinOptions> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Services.Configure(configuration);

            return this;
        }

        /// <summary>
        /// Disables the transport security requirement (HTTPS) during development.
        /// </summary>
        /// <returns>The <see cref="OpenIddictServerOwinBuilder"/>.</returns>
        public OpenIddictServerOwinBuilder DisableTransportSecurityRequirement()
            => Configure(options => options.DisableTransportSecurityRequirement = true);

        /// <summary>
        /// Enables the pass-through mode for the OpenID Connect authorization endpoint.
        /// When the pass-through mode is used, OpenID Connect requests are initially handled by OpenIddict.
        /// Once validated, the rest of the request processing pipeline is invoked, so that OpenID Connect requests
        /// can be handled at a later stage (in a custom middleware or in a MVC controller, for instance).
        /// </summary>
        /// <returns>The <see cref="OpenIddictServerOwinBuilder"/>.</returns>
        public OpenIddictServerOwinBuilder EnableAuthorizationEndpointPassthrough()
            => Configure(options => options.EnableAuthorizationEndpointPassthrough = true);

        /// <summary>
        /// Enables error pass-through support, so that the rest of the request processing pipeline is
        /// automatically invoked when returning an error from the interactive authorization and logout endpoints.
        /// When this option is enabled, special logic must be added to these actions to handle errors, that can be
        /// retrieved using <see cref="OpenIddictServerOwinHelpers.GetOpenIddictServerResponse(IOwinContext)"/>
        /// </summary>
        /// <returns>The <see cref="OpenIddictServerOwinBuilder"/>.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public OpenIddictServerOwinBuilder EnableErrorPassthrough()
            => Configure(options => options.EnableErrorPassthrough = true);

        /// <summary>
        /// Enables the pass-through mode for the OpenID Connect token endpoint.
        /// When the pass-through mode is used, OpenID Connect requests are initially handled by OpenIddict.
        /// Once validated, the rest of the request processing pipeline is invoked, so that OpenID Connect requests
        /// can be handled at a later stage (in a custom middleware or in a MVC controller, for instance).
        /// </summary>
        /// <returns>The <see cref="OpenIddictServerOwinBuilder"/>.</returns>
        public OpenIddictServerOwinBuilder EnableTokenEndpointPassthrough()
            => Configure(options => options.EnableTokenEndpointPassthrough = true);

        /// <summary>
        /// Enables the pass-through mode for the OpenID Connect userinfo endpoint.
        /// When the pass-through mode is used, OpenID Connect requests are initially handled by OpenIddict.
        /// Once validated, the rest of the request processing pipeline is invoked, so that OpenID Connect requests
        /// can be handled at a later stage (in a custom middleware or in a MVC controller, for instance).
        /// </summary>
        /// <returns>The <see cref="OpenIddictServerOwinBuilder"/>.</returns>
        public OpenIddictServerOwinBuilder EnableUserinfoEndpointPassthrough()
            => Configure(options => options.EnableUserinfoEndpointPassthrough = true);

        /// <summary>
        /// Enables request caching, so that both authorization and logout requests
        /// are automatically stored in the distributed cache, which allows flowing
        /// large payloads across requests. Enabling this option is recommended
        /// when using external authentication providers or when large GET or POST
        /// OpenID Connect authorization requests support is required.
        /// </summary>
        /// <returns>The <see cref="OpenIddictServerOwinBuilder"/>.</returns>
        public OpenIddictServerOwinBuilder EnableRequestCaching()
            => Configure(options => options.EnableRequestCaching = true);

        /// <summary>
        /// Sets the caching policy used to determine how long the authorization and
        /// end session requests should be cached by the distributed cache implementation.
        /// Note: the specified policy is only used when request caching is explicitly enabled.
        /// </summary>
        /// <param name="policy">The request caching policy.</param>
        /// <returns>The <see cref="OpenIddictServerOwinBuilder"/>.</returns>
        public OpenIddictServerOwinBuilder SetRequestCachingPolicy([NotNull] DistributedCacheEntryOptions policy)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            return Configure(options => options.RequestCachingPolicy = policy);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, false.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals([CanBeNull] object obj) => base.Equals(obj);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();
    }
}
