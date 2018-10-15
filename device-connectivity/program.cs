// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
using Microsoft.Extensions.Configuration.Binder;

namespace Microsoft.Azure.DigitalTwins.Samples
{
    class Program
    {
        private static Random rnd = new Random();
        private static IConfigurationSection settings;
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                Console.WriteLine("Usage: dotnet run\nNo arguments are supported");
                return;
            }

            settings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("Settings");

            try
            {
                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(settings["DeviceConnectionString"]);

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

        static Func<string> CreateGetRandomSensorReading(string sensorDataType, int iteration)
        {
            switch (sensorDataType)
            {
                default:
                    throw new Exception($"Unsupported configuration: SensorDataType, '{sensorDataType}'. Please check your appsettings.json.");
                case "Motion":
                    if (iteration % 6 < 3)
                        return () => "false";
                    else
                        return () => "true";

                case "Temperature":
                    return () => rnd.Next(70, 100).ToString(CultureInfo.InvariantCulture);
                case "CarbonDioxide":
                    if (iteration % 6 < 3)
                        return () => rnd.Next(800, 1000).ToString(CultureInfo.InvariantCulture);
                    else
                        return () => rnd.Next(1000, 1100).ToString(CultureInfo.InvariantCulture);
            }
        }

        static async Task SendEvent(DeviceClient deviceClient)
        {
            var serializer = new DataContractJsonSerializer(typeof(CustomTelemetryMessage));

            var sensors = settings.GetSection("Sensors").Get<Sensor[]>();

            var delayPerMessageSend = int.Parse(settings["MessageIntervalInSeconds"]);
            var countOfSendsPerIteration = sensors.Length;
            var maxSecondsToRun = 15 * 60;
            var maxIterations = maxSecondsToRun / countOfSendsPerIteration / delayPerMessageSend;
            var curIteration = 0;

            do {
                foreach (var sensor in sensors)
                {
                    var getRandomSensorReading = CreateGetRandomSensorReading(sensor.DataType, curIteration);
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
                        eventMessage.Properties.Add("DigitalTwins-SensorHardwareId", $"{sensor.HardwareId}");
                        eventMessage.Properties.Add("CreationTimeUtc", DateTime.UtcNow.ToString("o"));
                        eventMessage.Properties.Add("x-ms-client-request-id", Guid.NewGuid().ToString());

                        Console.WriteLine($"\t{DateTime.UtcNow.ToLocalTime()}> Sending message: {Encoding.UTF8.GetString(eventMessage.GetBytes())} Properties: {{ {eventMessage.Properties.Aggregate(new StringBuilder(), (sb, x) => sb.Append($"'{x.Key}': '{x.Value}',"), sb => sb.ToString())} }}");

                        await deviceClient.SendEventAsync(eventMessage);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(delayPerMessageSend));

            } while (++curIteration < maxIterations);

            Console.WriteLine($"Finished sending {curIteration} events (per sensor type)");
        }
    }

    public class Sensor
    {
        public string DataType { get; set; }
        public string HardwareId { get; set; }
    }
}