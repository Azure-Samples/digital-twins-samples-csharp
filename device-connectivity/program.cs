// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.DigitalTwins.Samples.Models;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    class Program
    {
        private static Random rnd = new Random();
        private static string hardwareId;
        private static IConfigurationSection settings;
        static void Main(string[] args)
        {
            settings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Settings");

            hardwareId = GetMacAddress();

            Console.WriteLine($"INFO: Your hardware ID is: {hardwareId}");

            var topologyClient = new TopologyClient(settings["ManagementApiUrl"], settings["SasToken"]);
            var device = topologyClient.GetDeviceForHardwareId(hardwareId).Result;
            if (device == null)
            {
                Console.WriteLine("ERROR: Failed to retrieve device from topology API. Please refer to documentation for pre-provisioning necessary metadata.");
                return;
            }

            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(device.ConnectionString);

                if (deviceClient == null)
                {
                    Console.WriteLine("ERROR: Failed to create DeviceClient!");
                    return;
                }
                
                SendEvent(deviceClient).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXIT: Unexpected error: {0}", ex.Message);
            }
        }

        static Func<string> CreateGetRandomSensorReading(string sensorDataType)
        {
            switch (sensorDataType)
            {
                default:
                    throw new Exception($"Unsupported configuration: SensorDataType, '{sensorDataType}'. Please check your appsettings.json.");
                case "Motion":
                    return () => rnd.Next(0, 2) == 0 ? "false" : "true";
                case "Temperature":
                    return () => rnd.Next(70, 100).ToString(CultureInfo.InvariantCulture);
                case "CarbonDioxide":
                    return () => rnd.Next(950, 1300).ToString(CultureInfo.InvariantCulture);
            }
        }

        static async Task SendEvent(DeviceClient deviceClient)
        {
            var serializer = new DataContractJsonSerializer(typeof(CustomTelemetryMessage));

            var sensorDataTypes = settings["SensorDataTypes"].Split(',');

            while (true)
            {
                foreach (var sensorDataType in sensorDataTypes)
                {
                    var getRandomSensorReading = CreateGetRandomSensorReading(sensorDataType);
                    var telemetryMessage = new CustomTelemetryMessage()
                    {
                        SensorValue = getRandomSensorReading(),
                    };

                    using (var stream = new MemoryStream())
                    {
                        serializer.WriteObject(stream, telemetryMessage);
                        var byteArray = stream.ToArray();
                        Message eventMessage = new Message(byteArray);
                        eventMessage.Properties.Add("DigitalTwins-Telemetry", "1.0");
                        eventMessage.Properties.Add("DigitalTwins-SensorHardwareId", $"{hardwareId}-{sensorDataType}");
                        eventMessage.Properties.Add("CreationTimeUtc", DateTime.UtcNow.ToString("o"));
                        eventMessage.Properties.Add("CorrelationId", Guid.NewGuid().ToString());

                        Console.WriteLine($"\t{DateTime.UtcNow.ToLocalTime()}> Sending message: {Encoding.UTF8.GetString(eventMessage.GetBytes())} Properties: {{ {eventMessage.Properties.Aggregate(new StringBuilder(), (sb, x) => sb.Append($"'{x.Key}': '{x.Value}',"), sb => sb.ToString())} }}");

                        await deviceClient.SendEventAsync(eventMessage);
                        await Task.Delay(int.Parse(settings["MessageIntervalInMilliSeconds"]));
                    }
                }
            }
        }

        private static string GetMacAddress()
        {
            foreach(var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up && nic.Speed > 0)
                {
                    var physAddress = nic.GetPhysicalAddress().ToString();
                    if (physAddress.Length > 0)
                    {
                        return physAddress;
                    }
                }
            }

            throw new Exception("No hardware address found.");
        }
    }
}