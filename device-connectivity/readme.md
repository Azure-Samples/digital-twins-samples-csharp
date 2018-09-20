# Digital Twins Device Connectivity Sample

This sample provides a small framework to connect a device to Digital Twins providing
sample sensory data.

## Configuring the app

Edit [appsettings.json](./appsettings.json) to fill in the following values:
* `ManagementApiUrl`
* `SasToken`

>Note: You have already generated a SAS Token from a configured Key on the Keystore API. To generate a valid token, you will need to
know your device's MAC address. Remove all columns (:) from the address and capitalize the value. E.g.: If your MAC address is `18:36:ba:0c:85:13`, the expected value would be `1836BA0C8513`. Please ensure this value is also reflected on the `HardwareId` property of your Topology Device and the `Status` property is set to `Active`. If you need to override the identifier you can simply edit [Program.cs](./Program.cs) and manually set the `hardwareId` variable.

Configuration example:
```
{
  "Settings": {
    "ManagementApiUrl": "https://name.westcentralus.azuresmartspaces.net/management/",
    "SasToken": "SharedAccessSignature id=1836BA0C8513&se=31556995200&kv=1&sig=...",
    "MessageIntervalInMilliSeconds": 5000,
    "SensorDataTypes": "Temperature,Motion,CarbonDioxide"
  }
}
```  

## Building the app

* [Install .NET Core SDK](https://www.microsoft.com/net/core) on your execution platform
* Run `dotnet restore`
* Run `dotnet build`

## Executing the app

* Run `dotnet run`

>Note: Here is how you can run this app on a Raspberry Pi

## Running your App on a Raspberry Pi

### Building

Choose `win-arm` or `linux-arm` for your platform:
* Run `dotnet publish -r <platform>`

Under ./bin/Debug/netcoreapp2.0/<runtime identifier>/publish or .\bin\Debug\netcoreapp2.0\<runtime identifier>\publish you will see the whole self contained app that you need to copy to your Raspberry Pi.

### Windows 10 IoT

* Run on device by invoking the executable

### Linux

* Install [Linux](https://www.raspberrypi.org/downloads/) on your Pi.
* Install the [platform dependencies from your distro's package manager](https://github.com/dotnet/core/blob/master/Documentation/prereqs.md) for .NET Core.
* `chmod 755 ./device-connectivity`

## Customizing the app to your needs

This app compiles a sample telemetry message with some randomly generated data. You can customize the message format as well as the payload by modifying [Models/CustomTelemetryMessage.cs](./Models/CustomTelemetryMessage.cs). The data contract of the model can be changed to any serializable format or you can choose to compile your own payload by generating a byte array or stream that can be passed to  [Message(byte[] byteArray)](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.devices.client.message.-ctor?view=azure-dotnet#Microsoft_Azure_Devices_Client_Message__ctor_System_Byte___), as found in the `SendEvent` method. You will have to maintain a set of properties to ensure your message keeps getting routed appropriately, as listed below.

### Telemetry Properties

While the payload contents of a `Message` can be arbitrary data, up to 256kb in size, there are a few requirements on expected [Message.Properties](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.devices.client.message.properties?view=azure-dotnet). The following is the list of required and optional properties supported by the system:

| Property Name | Value | Required | Description |
|---------------|-------|----------|-------------|
| DigitalTwins-Telemetry | 1.0 | yes | A constant value that identifies a message to the system |
| DigitalTwins-SensorHardwareId | `string(72)` | yes | A unique identifier pointing to a Sensor in the topology for which the `Message` is meant for. This value must match an object's `HardwareId` property for the system to process it. E.g.: `00FF0643BE88-PIR` |
| CreationTimeUtc | `string` | no | An [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) formatted date string identifying the sampling time of the payload. E.g.: `2018-09-20T07:35:00.8587882-07:00` |
| CorrelationId | `string` | no | A `uuid` formatted as a string that can be used to trace events across the system. E.g.: `cec16751-ab27-405d-8fe6-c68e1412ce1f`
