// <copyright file="Program.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace Sample.OnlineMeeting
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Graph;
    using Microsoft.Identity.Client;

    /// <summary>
    /// Default program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets the online meeting asynchronous.
        /// </summary>
        /// <param name="applicationConfig">Configuration of the app containing app parameters.</param>
        /// <param name="authenticationProvider">Instance of Auth provider helper class.</param>
        /// <returns> The onlinemeeting details. </returns>
        public static async Task<Microsoft.Graph.OnlineMeeting> GetOnlineMeetingAsync(ApplicationConfig applicationConfig, IAuthenticationProvider authenticationProvider)
        {
            var onlineMeeting = new OnlineMeeting(
                authenticationProvider,
                new Uri(applicationConfig.MicrosoftGraphBaseEndpoint));

            var meetingDetails = await onlineMeeting.GetOnlineMeetingAsync(
                applicationConfig.TenantId,
                applicationConfig.MeetingId,
                default(Guid))
                .ConfigureAwait(false);

            Console.WriteLine(meetingDetails.Id);
            Console.WriteLine(meetingDetails.ChatInfo.ThreadId);

            return meetingDetails;
        }

        /// <summary>
        /// Creates the online meeting asynchronous.
        /// </summary>
        /// <param name="applicationConfig">Configuration of the app containing app parameters.</param>
        /// <param name="authenticationProvider">Graph client for making graph calls.</param>
        /// <returns> The newly created onlinemeeting. </returns>
        public static async Task<Microsoft.Graph.OnlineMeeting> CreateOnlineMeetingAsync(ApplicationConfig applicationConfig, IAuthenticationProvider authenticationProvider)
        {
            var onlineMeeting = new OnlineMeeting(
                authenticationProvider,
                new Uri(applicationConfig.MicrosoftGraphBaseEndpoint));

            var meetingDetails = await onlineMeeting.CreateOnlineMeetingAsync(
                applicationConfig.TenantId,
                applicationConfig.OrganizerId,
                default(Guid))
                .ConfigureAwait(false);

            Console.WriteLine(meetingDetails.Id);
            Console.WriteLine(meetingDetails.ChatInfo.ThreadId);

            return meetingDetails;
        }

        /// <summary>
        /// The Main entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            ApplicationConfig appConfiguration = ApplicationConfig.ReadFromJsonFile("appsettings.json");

            ConfidentialClientApplication confidentialClientApplication = new ConfidentialClientApplication(
                appConfiguration.ClientId,
                appConfiguration.Authority,
                appConfiguration.RedirectUrl,
                new ClientCredential(appConfiguration.ClientSecret),
                new TokenCache(),
                new TokenCache());

            IAuthenticationProvider authenticationProvider = new MsalAuthenticationProvider(
                confidentialClientApplication,
                new string[] { "https://graph.microsoft.com/.default" });

            Task.Run(async () =>
            {
                try
                {
                    var meetingDetails = await GetOnlineMeetingAsync(appConfiguration, authenticationProvider).ConfigureAwait(false);
                    var createdMeetingDetails = await CreateOnlineMeetingAsync(appConfiguration, authenticationProvider).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });

            Console.ReadKey();
        }
    }
}
