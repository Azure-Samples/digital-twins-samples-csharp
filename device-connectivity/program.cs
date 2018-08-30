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
        public static Device DeviceInfo { get; set; }
        public static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.dev.json", optional: true)
                .Build();

            var hardwareId = GetMacAddress();

            Console.WriteLine($"Your hardware ID is: {hardwareId}");

            var topologyClient = new TopologyClient(Configuration["ManagementApiUrl"], Configuration["SasToken"]);
            DeviceInfo = topologyClient.GetDeviceForHardwareId(hardwareId).Result;
            if (DeviceInfo == null)
            {
                Console.WriteLine("ERROR: Could not retrieve device information.");
                return;
            }

            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(DeviceInfo.ConnectionString);

                if (deviceClient == null)
                {
                    Console.WriteLine("Failed to create DeviceClient!");
                }
                else
                {
                    SendEvent(deviceClient).Wait();
                }

                Console.WriteLine("Exited!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in sample: {0}", ex.Message);
            }
        }

        static Func<string> CreateGetRandomSensorReading(string sensorDataType)
        {
            switch (sensorDataType)
            {
                default:
                    throw new Exception($"Unsupported configuration: SensorDataType, '{sensorDataType}'");
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
            var serializer = new DataContractJsonSerializer(typeof(TelemetryMessage));

            var sensorDataTypes = Configuration["SensorDataTypes"].Split(',');

            while (true)
            {
                foreach (var sensorDataType in sensorDataTypes)
                {
                    var getRandomSensorReading = CreateGetRandomSensorReading(sensorDataType);

                    var sensor = DeviceInfo.Sensors.FirstOrDefault(x => (x.DataType == sensorDataType));
                    if (sensor == null)
                    {
                        throw new Exception($"No preconfigured Sensor for DataType '{sensorDataType}' found.");
                    }

                    var telemetryMessage = new TelemetryMessage()
                    {
                        SensorId=sensor.Id,
                        SensorReading=getRandomSensorReading(),
                        EventTimestamp=DateTime.UtcNow.ToString("o"),
                        SensorType=sensor.Type,
                        SensorDataType=sensor.DataType
                    };

                    using (var stream = new MemoryStream())
                    {
                        serializer.WriteObject(stream, telemetryMessage);
                        var binaryMessage = stream.ToArray();
                        Message eventMessage = new Message(binaryMessage);
                        eventMessage.Properties.Add("Sensor", "");
                        eventMessage.Properties.Add("MessageVersion", "1.0");
                        eventMessage.Properties.Add("x-ms-flighting-udf-execution-manually-enabled", "true");
                        Console.WriteLine($"\t{DateTime.UtcNow.ToLocalTime()}> Sending message: {Encoding.ASCII.GetString(binaryMessage)}");

                        await deviceClient.SendEventAsync(eventMessage).ConfigureAwait(false);
                    }
                }

                Thread.Sleep(int.Parse(Configuration["MessageIntervalInMilliSeconds"]));
            }
        }

        private static string GetMacAddress()
        {
            foreach(var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                var physAddress = nic.GetPhysicalAddress().ToString();
                if (physAddress.Length > 0)
                {
                    return physAddress;
                }
            }

            throw new Exception("No hardware address found.");
        }
    }
}