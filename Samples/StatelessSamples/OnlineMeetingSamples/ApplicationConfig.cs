// <copyright file="ApplicationConfig.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace Sample.OnlineMeeting
{
    using System.Globalization;
    using System.IO;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Description of the configuration of an AzureAD public client application (desktop/mobile application). This should
    /// match the application registration done in the Azure portal.
    /// </summary>
    public class ApplicationConfig
    {
        /// <summary>
        /// Gets or sets instance of Azure AD, for example public Azure or a Sovereign cloud (Azure China, Germany, US government, etc ...)
        /// </summary>
        public string Instance { get; set; } = "https://login.microsoftonline.com/{0}";

        /// <summary>
        /// Gets or sets the TenantId
        /// The Tenant is:
        /// - either the tenant ID of the Azure AD tenant in which this application is registered (a guid)
        /// or a domain name associated with the tenant
        /// - or 'organizations' (for a multi-tenant application).
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the guid used by the application to uniquely identify itself to Azure AD.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the secret used by the application to uniquely identify itself to Azure AD.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets the URL of the authority.
        /// </summary>
        public string Authority
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, this.Instance, this.TenantId);
            }
        }

        /// <summary>
        /// Gets or sets the URL provided by the app on authentication with AD for redirect on successfull auth.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets base URL for the Microsoft Graph endpoint (depends on the Azure Cloud).
        /// </summary>
        public string MicrosoftGraphBaseEndpoint { get; set; }

        /// <summary>
        /// Gets or sets meetingId of the online meeting.
        /// </summary>
        public string MeetingId { get; set; }

        /// <summary>
        /// Gets or sets the OrganiserId of the online meeting.
        /// </summary>
        public string OrganizerId { get; set; }

        /// <summary>
        /// Reads the configuration from a json file.
        /// </summary>
        /// <param name="path">Path to the configuration json file.</param>
        /// <returns>AuthenticationConfig read from the json file.</returns>
        public static ApplicationConfig ReadFromJsonFile(string path)
        {
            IConfigurationRoot configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

            configuration = builder.Build();

            return configuration.Get<ApplicationConfig>();
        }
    }
}
