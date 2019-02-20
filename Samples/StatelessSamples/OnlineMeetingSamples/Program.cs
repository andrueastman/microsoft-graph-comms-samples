// <copyright file="Program.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// </copyright>

namespace Sample.OnlineMeeting
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Default program class.
    /// </summary>
    public class Program
    {
        private static string appSecret = "=V}.LD&e]uWT+/n{Hk}$x/>^:-$^W]Y$xDK=4#|?514&9}G=aYy:TrC^";
        private static string appId = "a993dc16-d4e0-4647-bd71-3103db2673f1";

        // private static string meetingId = "_19:meeting_OGY4NTUxYTQtZWQ4Ny00ZDMyLWFkMWEtNzZhMThiNjI1YTZj@thread.v2";
        private static string tenantId = "703acd3b-f147-4de6-81dc-c520e024626d";
        private static string organizerID = "d64e8a59-7214-4479-8d82-932b2929c0d0";

        private static Uri graphUri = new Uri("https://graph.microsoft.com/beta/");

        /// <summary>
        /// Gets the online meeting asynchronous.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <returns> The onlinemeeting details. </returns>
        public static async Task<Microsoft.Graph.OnlineMeeting> GetOnlineMeetingAsync(string tenantId, string meetingId)
        {
            var onlineMeeting = new OnlineMeeting(
                        new RequestAuthenticationProvider(appId, appSecret),
                        graphUri);

            var meetingDetails = await onlineMeeting.GetOnlineMeetingAsync(tenantId, meetingId, default(Guid)).ConfigureAwait(false);

            Console.WriteLine(meetingDetails.Id);
            Console.WriteLine(meetingDetails.ChatInfo.ThreadId);

            return meetingDetails;
        }

        /// <summary>
        /// Creates the online meeting asynchronous.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="organizerId">The organizer identifier.</param>
        /// <returns> The newly created onlinemeeting. </returns>
        public static async Task<Microsoft.Graph.OnlineMeeting> CreateOnlineMeetingAsync(string tenantId, string organizerId)
        {
            var onlineMeeting = new OnlineMeeting(
                        new RequestAuthenticationProvider(appId, appSecret),
                        graphUri);

            var meetingDetails = await onlineMeeting.CreateOnlineMeetingAsync(tenantId, organizerId, default(Guid)).ConfigureAwait(false);

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
            Task.Run(async () =>
            {
                try
                {
                    // var meetingDetails = await GetOnlineMeetingAsync(tenantId, meetingId).ConfigureAwait(false);
                    var createdMeetingDetails = await CreateOnlineMeetingAsync(tenantId, organizerID).ConfigureAwait(false);
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
