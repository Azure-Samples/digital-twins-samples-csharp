// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    public static partial class Actions
    {
        // Prints out and returns spaces with the occupany property key set
        public static async Task GetAvailableAndFreshSpaces(HttpClient httpClient)
        {
            Console.WriteLine("Polling spaces with 'AvailableAndFresh' value type");

            var maxGets = 30;
            for (var curGets = 0; curGets < maxGets; ++curGets)
            {
                var (spaces, response) = await GetManagementItemsAsync<Models.Space>(httpClient, "spaces", "includes=values");
                if (spaces == null)
                {
                    var content = await response.Content?.ReadAsStringAsync();
                    Console.WriteLine($"ERROR: GET spaces?includes=values failed with: {(int)response.StatusCode}, {response.StatusCode} {content}");
                    break;
                }

                var availableAndFreshSpaces = spaces.Where(s => s.Values != null && s.Values.Any(v => v.Type == "AvailableAndFresh"));
                if (availableAndFreshSpaces.Any())
                {
                    var availableAndFreshDisplay = availableAndFreshSpaces
                        .Select(s => GetDisplayValues(s))
                        .Aggregate((acc, cur) => acc + "\n" + cur);
                    Console.WriteLine($"{availableAndFreshDisplay}");
                }
                else
                {
                    Console.WriteLine("Unable to find a space with value type 'AvailableAndFresh'");
                }

                await Task.Delay(TimeSpan.FromSeconds(4));
            }
        }

        private static async Task<(IEnumerable<T>, HttpResponseMessage)> GetManagementItemsAsync<T>(
            HttpClient httpClient,
            string queryItem,
            string queryParams)
        {
            var response = await httpClient.GetAsync($"{queryItem}?{queryParams}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var objects = JsonConvert.DeserializeObject<IEnumerable<T>>(content);
                return (objects, response);
            }

            return (null, response);
        }

        private static string GetDisplayValues(Models.Space space)
        {
            var spaceValue = space.Values.First(v => v.Type == "AvailableAndFresh");
            return $"Name: {space.Name}\nId: {space.Id}\nTimestamp: {spaceValue.Timestamp}\nValue: {spaceValue.Value}\n";
        }
    }
}