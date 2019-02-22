// <copyright file="MsalAuthenticationProvider.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace Sample.OnlineMeeting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    using Microsoft.Identity.Client;

    /// <summary>
    /// This class encapsulates the details of getting a token from MSAL and exposes it via the
    /// IAuthenticationProvider interface so that GraphServiceClient or AuthHandler can use it.
    /// A significantly enhanced version of this class will in the future be available from
    /// the GraphSDK team.  It will supports all the types of Client Application as defined by MSAL. authentication provider.
    /// </summary>
    public class MsalAuthenticationProvider : IAuthenticationProvider
    {
        /// <summary>
        /// The local instance of a <see cref="ConfidentialClientApplication"/>.
        /// </summary>
        private ConfidentialClientApplication clientApplication;

        /// <summary>
        /// The local instance of scopes used by the application.
        /// </summary>
        private IEnumerable<string> scopes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsalAuthenticationProvider"/> class.
        /// </summary>
        /// <param name="clientApplication">Client application to be used by the class.</param>
        /// <param name="scopes">>Scopes to be used by the class.</param>
        public MsalAuthenticationProvider(ConfidentialClientApplication clientApplication, IEnumerable<string> scopes)
        {
            this.clientApplication = clientApplication;
            this.scopes = scopes;
        }

        /// <summary>
        /// Update HttpRequestMessage with credentials.
        /// </summary>
        /// <param name="request">Http request to update with  credentials.</param>
        /// <returns>A <see cref="Task"/> that has authenticates the request.</returns>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            var token = await this.GetTokenAsync().ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        /// <summary>
        /// Acquire Token.
        /// </summary>
        /// <returns>A <see cref="Task"/> with the string token to authenticate requests.</returns>
        public async Task<string> GetTokenAsync()
        {
            AuthenticationResult authResult;
            authResult = await this.clientApplication.AcquireTokenForClientAsync(this.scopes).ConfigureAwait(false);
            return authResult.AccessToken;
        }
    }
}