// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public class AppSettings {
        public string AADInstance { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; }
        public string Tenant { get; set; }
        public string BaseUrl { get; set; }
        public string Authority => AADInstance + Tenant;

        public static AppSettings Load()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.dev.json", optional: true)
                .Build()
                .Get<AppSettings>();

            // Sanitize input

            // This is because httpClient will behave differently if the
            // passed in Uri has a trailing slash when using GetAsync.
            appSettings.BaseUrl = EnsureTrailingSlash(appSettings.BaseUrl);

            return appSettings;
        }

        private static string EnsureTrailingSlash(string baseUrl)
            => baseUrl.Length == 0 || baseUrl[baseUrl.Length-1] == '/'
                ? baseUrl
                : baseUrl + '/';
    }
}