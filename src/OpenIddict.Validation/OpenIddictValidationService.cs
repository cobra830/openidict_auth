﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Validation.OpenIddictValidationEvents;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace OpenIddict.Validation
{
    public class OpenIddictValidationService
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Creates a new instance of the <see cref="OpenIddictValidationService"/> class.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        public OpenIddictValidationService([NotNull] IServiceProvider provider)
            => _provider = provider;

        /// <summary>
        /// Retrieves the OpenID Connect server configuration from the specified address.
        /// </summary>
        /// <param name="address">The address of the remote metadata endpoint.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>The OpenID Connect server configuration retrieved from the remote server.</returns>
        public async ValueTask<OpenIdConnectConfiguration> GetConfigurationAsync(
            [NotNull] Uri address, CancellationToken cancellationToken = default)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!address.IsAbsoluteUri)
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID1143), nameof(address));
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Note: this service is registered as a singleton service. As such, it cannot
            // directly depend on scoped services like the validation provider. To work around
            // this limitation, a scope is manually created for each method to this service.
            var scope = _provider.CreateScope();

            // Note: a try/finally block is deliberately used here to ensure the service scope
            // can be disposed of asynchronously if it implements IAsyncDisposable.
            try
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<IOpenIddictValidationDispatcher>();
                var factory = scope.ServiceProvider.GetRequiredService<IOpenIddictValidationFactory>();
                var transaction = await factory.CreateTransactionAsync();

                var request = new OpenIddictRequest();
                request = await PrepareConfigurationRequestAsync();
                request = await ApplyConfigurationRequestAsync();
                var response = await ExtractConfigurationResponseAsync();

                var configuration = await HandleConfigurationResponseAsync();
                if (configuration == null)
                {
                    throw new InvalidOperationException(SR.GetResourceString(SR.ID1144));
                }

                return configuration;

                async ValueTask<OpenIddictRequest> PrepareConfigurationRequestAsync()
                {
                    var context = new PrepareConfigurationRequestContext(transaction)
                    {
                        Address = address,
                        Request = request
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1147(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Request;
                }

                async ValueTask<OpenIddictRequest> ApplyConfigurationRequestAsync()
                {
                    var context = new ApplyConfigurationRequestContext(transaction)
                    {
                        Request = request
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1148(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Request;
                }

                async ValueTask<OpenIddictResponse> ExtractConfigurationResponseAsync()
                {
                    var context = new ExtractConfigurationResponseContext(transaction)
                    {
                        Request = request
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1149(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Response;
                }

                async ValueTask<OpenIdConnectConfiguration> HandleConfigurationResponseAsync()
                {
                    var context = new HandleConfigurationResponseContext(transaction)
                    {
                        Request = request,
                        Response = response
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1150(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Configuration;
                }
            }

            finally
            {
                if (scope is IAsyncDisposable disposable)
                {
                    await disposable.DisposeAsync();
                }

                else
                {
                    scope.Dispose();
                }
            }
        }

        /// <summary>
        /// Retrieves the security keys exposed by the specified JWKS endpoint.
        /// </summary>
        /// <param name="address">The address of the remote metadata endpoint.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>The security keys retrieved from the remote server.</returns>
        public async ValueTask<JsonWebKeySet> GetSecurityKeysAsync(
            [NotNull] Uri address, CancellationToken cancellationToken = default)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!address.IsAbsoluteUri)
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID1143), nameof(address));
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Note: this service is registered as a singleton service. As such, it cannot
            // directly depend on scoped services like the validation provider. To work around
            // this limitation, a scope is manually created for each method to this service.
            var scope = _provider.CreateScope();

            // Note: a try/finally block is deliberately used here to ensure the service scope
            // can be disposed of asynchronously if it implements IAsyncDisposable.
            try
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<IOpenIddictValidationDispatcher>();
                var factory = scope.ServiceProvider.GetRequiredService<IOpenIddictValidationFactory>();
                var transaction = await factory.CreateTransactionAsync();

                var request = new OpenIddictRequest();
                request = await PrepareCryptographyRequestAsync();
                request = await ApplyCryptographyRequestAsync();

                var response = await ExtractCryptographyResponseAsync();

                var keys = await HandleCryptographyResponseAsync();
                if (keys == null)
                {
                    throw new InvalidOperationException(SR.GetResourceString(SR.ID1146));
                }

                return keys;

                async ValueTask<OpenIddictRequest> PrepareCryptographyRequestAsync()
                {
                    var context = new PrepareCryptographyRequestContext(transaction)
                    {
                        Address = address,
                        Request = request
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1151(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Request;
                }

                async ValueTask<OpenIddictRequest> ApplyCryptographyRequestAsync()
                {
                    var context = new ApplyCryptographyRequestContext(transaction)
                    {
                        Request = request
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1152(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Request;
                }

                async ValueTask<OpenIddictResponse> ExtractCryptographyResponseAsync()
                {
                    var context = new ExtractCryptographyResponseContext(transaction)
                    {
                        Request = request
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1153(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Response;
                }

                async ValueTask<JsonWebKeySet> HandleCryptographyResponseAsync()
                {
                    var context = new HandleCryptographyResponseContext(transaction)
                    {
                        Request = request,
                        Response = response
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1154(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.SecurityKeys;
                }
            }

            finally
            {
                if (scope is IAsyncDisposable disposable)
                {
                    await disposable.DisposeAsync();
                }

                else
                {
                    scope.Dispose();
                }
            }
        }

        /// <summary>
        /// Sends an introspection request to the specified address and returns the corresponding principal.
        /// </summary>
        /// <param name="address">The address of the remote metadata endpoint.</param>
        /// <param name="token">The token to introspect.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>The claims principal created from the claim retrieved from the remote server.</returns>
        public ValueTask<ClaimsPrincipal> IntrospectTokenAsync(
            [NotNull] Uri address, [NotNull] string token, CancellationToken cancellationToken = default)
            => IntrospectTokenAsync(address, token, type: null, cancellationToken);

        /// <summary>
        /// Sends an introspection request to the specified address and returns the corresponding principal.
        /// </summary>
        /// <param name="address">The address of the remote metadata endpoint.</param>
        /// <param name="token">The token to introspect.</param>
        /// <param name="type">The token type to introspect.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>The claims principal created from the claim retrieved from the remote server.</returns>
        public async ValueTask<ClaimsPrincipal> IntrospectTokenAsync(
            [NotNull] Uri address, [NotNull] string token,
            [CanBeNull] string type, CancellationToken cancellationToken = default)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (!address.IsAbsoluteUri)
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID1143), nameof(address));
            }

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID1155), nameof(token));
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Note: this service is registered as a singleton service. As such, it cannot
            // directly depend on scoped services like the validation provider. To work around
            // this limitation, a scope is manually created for each method to this service.
            var scope = _provider.CreateScope();

            // Note: a try/finally block is deliberately used here to ensure the service scope
            // can be disposed of asynchronously if it implements IAsyncDisposable.
            try
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<IOpenIddictValidationDispatcher>();
                var factory = scope.ServiceProvider.GetRequiredService<IOpenIddictValidationFactory>();
                var transaction = await factory.CreateTransactionAsync();

                var request = new OpenIddictRequest();
                request = await PrepareIntrospectionRequestAsync();
                request = await ApplyIntrospectionRequestAsync();
                var response = await ExtractIntrospectionResponseAsync();

                var principal = await HandleIntrospectionResponseAsync();
                if (principal == null)
                {
                    throw new InvalidOperationException(SR.GetResourceString(SR.ID1156));
                }

                return principal;

                async ValueTask<OpenIddictRequest> PrepareIntrospectionRequestAsync()
                {
                    var context = new PrepareIntrospectionRequestContext(transaction)
                    {
                        Address = address,
                        Request = request,
                        Token = token,
                        TokenType = type
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1157(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Request;
                }

                async ValueTask<OpenIddictRequest> ApplyIntrospectionRequestAsync()
                {
                    var context = new ApplyIntrospectionRequestContext(transaction)
                    {
                        Request = request
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1158(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Request;
                }

                async ValueTask<OpenIddictResponse> ExtractIntrospectionResponseAsync()
                {
                    var context = new ExtractIntrospectionResponseContext(transaction)
                    {
                        Request = request
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1159(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Response;
                }

                async ValueTask<ClaimsPrincipal> HandleIntrospectionResponseAsync()
                {
                    var context = new HandleIntrospectionResponseContext(transaction)
                    {
                        Request = request,
                        Response = response,
                        Token = token,
                        TokenType = type
                    };

                    await dispatcher.DispatchAsync(context);

                    if (context.IsRejected)
                    {
                        throw new OpenIddictExceptions.GenericException(
                            SR.FormatID1160(context.Error, context.ErrorDescription, context.ErrorUri),
                            context.Error, context.ErrorDescription, context.ErrorUri);
                    }

                    return context.Principal;
                }
            }

            finally
            {
                if (scope is IAsyncDisposable disposable)
                {
                    await disposable.DisposeAsync();
                }

                else
                {
                    scope.Dispose();
                }
            }
        }

        /// <summary>
        /// Validates the specified access token and returns the principal extracted from the token.
        /// </summary>
        /// <param name="token">The access token to validate.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> that can be used to abort the operation.</param>
        /// <returns>The principal containing the claims extracted from the token.</returns>
        public async ValueTask<ClaimsPrincipal> ValidateAccessTokenAsync(
            [NotNull] string token, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID1161), nameof(token));
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Note: this service is registered as a singleton service. As such, it cannot
            // directly depend on scoped services like the validation provider. To work around
            // this limitation, a scope is manually created for each method to this service.
            var scope = _provider.CreateScope();

            // Note: a try/finally block is deliberately used here to ensure the service scope
            // can be disposed of asynchronously if it implements IAsyncDisposable.
            try
            {
                var dispatcher = scope.ServiceProvider.GetRequiredService<IOpenIddictValidationDispatcher>();
                var factory = scope.ServiceProvider.GetRequiredService<IOpenIddictValidationFactory>();
                var transaction = await factory.CreateTransactionAsync();

                var context = new ProcessAuthenticationContext(transaction)
                {
                    Token = token,
                    TokenType = TokenTypeHints.AccessToken
                };

                await dispatcher.DispatchAsync(context);

                if (context.IsRejected)
                {
                    throw new OpenIddictExceptions.GenericException(
                        SR.FormatID1162(context.Error, context.ErrorDescription, context.ErrorUri),
                        context.Error, context.ErrorDescription, context.ErrorUri);
                }

                return context.Principal;
            }

            finally
            {
                if (scope is IAsyncDisposable disposable)
                {
                    await disposable.DisposeAsync();
                }

                else
                {
                    scope.Dispose();
                }
            }
        }
    }
}
