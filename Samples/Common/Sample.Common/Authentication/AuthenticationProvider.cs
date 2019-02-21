﻿// <copyright file="AuthenticationProvider.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace Sample.Common.Authentication
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Graph.Communications.Client.Authentication;
    using Microsoft.Graph.Communications.Common;
    using Microsoft.Graph.Communications.Common.Telemetry;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// The authentication provider for this bot instance.
    /// </summary>
    /// <seealso cref="IRequestAuthenticationProvider" />
    public class AuthenticationProvider : IRequestAuthenticationProvider
    {
        /// <summary>
        /// The application identifier.
        /// </summary>
        private readonly string appId;

        /// <summary>
        /// The application secret.
        /// </summary>
        private readonly string appSecret;

        /// <summary>
        /// The graph logger.
        /// </summary>
        private readonly IGraphLogger graphLogger;

        /// <summary>
        /// The open ID configuration refresh interval.
        /// </summary>
        private readonly TimeSpan openIdConfigRefreshInterval = TimeSpan.FromHours(2);

        /// <summary>
        /// The previous update timestamp for OpenIdConfig.
        /// </summary>
        private DateTime prevOpenIdConfigUpdateTimestamp = DateTime.MinValue;

        /// <summary>
        /// The open identifier configuration.
        /// </summary>
        private OpenIdConnectConfiguration openIdConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationProvider" /> class.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="appSecret">The application secret.</param>
        /// <param name="logger">The logger.</param>
        public AuthenticationProvider(string appId, string appSecret, IGraphLogger logger)
        {
            this.appId = appId.NotNullOrWhitespace(nameof(appId));
            this.appSecret = appSecret.NotNullOrWhitespace(nameof(appSecret));
            this.graphLogger = logger.NotNull(nameof(logger)).CreateShim(nameof(AuthenticationProvider));
        }

        /// <summary>
        /// Authenticates the specified request message.
        /// This method will be called any time there is an outbound request.
        /// In this case we are using the Microsoft.IdentityModel.Clients.ActiveDirectory library
        /// to stamp the outbound http request with the OAuth 2.0 token using an AAD application id
        /// and application secret.  Alternatively, this method can support certificate validation.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="tenant">The tenant.</param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        public async Task AuthenticateOutboundRequestAsync(HttpRequestMessage request, string tenant)
        {
            const string schema = "Bearer";
            const string replaceString = "{tenant}";
            const string oauthV2TokenLink = "https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";
            const string resource = "https://graph.microsoft.com";

            // If no tenant was specified, we craft the token link using the common tenant.
            // https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-v2-protocols#endpoints
            this.graphLogger.Info("Tenant ID: " + tenant);
            tenant = string.IsNullOrWhiteSpace(tenant) ? "common" : tenant;
            var tokenLink = oauthV2TokenLink.Replace(replaceString, tenant);

            this.graphLogger.Info("AuthenticationProvider: Refreshing OAuth token.");
            var context = new AuthenticationContext(tokenLink);
            var creds = new ClientCredential(this.appId, this.appSecret);

            AuthenticationResult result;
            try
            {
                result = await context.AcquireTokenAsync(resource, creds).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.graphLogger.Error(ex, $"Failed to generate token for client: {this.appId}");
                throw;
            }

            this.graphLogger.Info($"AuthenticationProvider: OAuth token cached. Expires in {result.ExpiresOn.Subtract(DateTimeOffset.UtcNow).TotalMinutes} minutes.");

            request.Headers.Authorization = new AuthenticationHeaderValue(schema, result.AccessToken);
        }

        /// <summary>
        /// Validates the request asynchronously.
        /// This method will be called any time we have an incoming request.
        /// Returning invalid result will trigger a Forbidden response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The <see cref="RequestValidationResult" /> structure.
        /// </returns>
        public async Task<RequestValidationResult> ValidateInboundRequestAsync(HttpRequestMessage request)
        {
            var token = request?.Headers?.Authorization?.Parameter;
            if (string.IsNullOrWhiteSpace(token))
            {
                return new RequestValidationResult { IsValid = false };
            }

            // Currently the service does not sign outbound request using AAD, instead it is signed
            // with a private certificate.  In order for us to be able to ensure the certificate is
            // valid we need to download the corresponding public keys from a trusted source.
            const string authDomain = "https://api.aps.skype.com/v1/.well-known/OpenIdConfiguration";
            if (this.openIdConfiguration == null || DateTime.Now > this.prevOpenIdConfigUpdateTimestamp.Add(this.openIdConfigRefreshInterval))
            {
                this.graphLogger.Info("Updating OpenID configuration");

                // Download the OIDC configuration which contains the JWKS
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager =
                    new ConfigurationManager<OpenIdConnectConfiguration>(
                        authDomain,
                        new OpenIdConnectConfigurationRetriever());
                this.openIdConfiguration = await configurationManager.GetConfigurationAsync(CancellationToken.None).ConfigureAwait(false);

                this.prevOpenIdConfigUpdateTimestamp = DateTime.Now;
            }

            // The incoming token should be issued by graph.
            var authIssuers = new[]
            {
                "https://graph.microsoft.com",
                "https://api.botframework.com",
            };

            // Configure the TokenValidationParameters.
            // Aet the Issuer(s) and Audience(s) to validate and
            // assign the SigningKeys which were downloaded from AuthDomain.
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidIssuers = authIssuers,
                ValidAudience = this.appId,
                IssuerSigningKeys = this.openIdConfiguration.SigningKeys,
            };

            ClaimsPrincipal claimsPrincipal;
            try
            {
                // Now validate the token. If the token is not valid for any reason, an exception will be thrown by the method
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                claimsPrincipal = handler.ValidateToken(token, validationParameters, out _);
            }

            // Token expired... should somehow return 401 (Unauthorized)
            // catch (SecurityTokenExpiredException ex)
            // Tampered token
            // catch (SecurityTokenInvalidSignatureException ex)
            // Some other validation error
            // catch (SecurityTokenValidationException ex)
            catch (Exception ex)
            {
                // Some other error
                this.graphLogger.Error(ex, $"Failed to validate token for client: {this.appId}.");
                return new RequestValidationResult() { IsValid = false };
            }

            const string ClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";
            var tenantClaim = claimsPrincipal.FindFirst(claim => claim.Type.Equals(ClaimType, StringComparison.Ordinal));

            if (string.IsNullOrEmpty(tenantClaim?.Value))
            {
                // No tenant claim given to us.  reject the request.
                return new RequestValidationResult { IsValid = false };
            }

            return new RequestValidationResult { IsValid = true, TenantId = tenantClaim.Value };
        }
    }
}