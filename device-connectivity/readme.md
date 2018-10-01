# Digital Twins Device Connectivity Sample

This sample provides a small framework to connect a device to Digital Twins providing
sample sensory data.

## Configuring the app

Edit your settings file to fill in the following values:
* `DeviceConnectionString`
* `Sensors`

### DeviceConnectionString

You can get the connection string by calling Management API's Devices controller. E.g. for a device with ID `22215ed9-91e8-40af-9d1b-a22727849393`:

```
GET https://<instance-name>.westcentralus.azuresmartspaces.net/management/api/v1.0/devices/22215ed9-91e8-40af-9d1b-a22727849393?includes=ConnectionString

{
    "name": "My Sample Device",
    "typeId": 2,
    "subtypeId": 1,
    "hardwareId": "00AABBCCDDEE",
    "spaceId": "ef46f69f-0b95-45ee-a41f-718b3ee4c355",
    "status": "Active",
    "id": "22215ed9-91e8-40af-9d1b-a22727849393",
    "connectionString": "Hostname=..."
}

```

Copy the `connectionString` over to the [appsettings.json](./appsettings.json) file:

```
{
  "Settings": {
    "DeviceConnectionString": "Hostname=...",
    ...
  }
}
```

### Sensors

An array of one or more Sensors you wish to send data for using this sample. You can get the DataType and HardwareId of each Sensor by calling Management API's Sensors controller. E.g. for a sensor with ID `c4cf2f41-edd6-4fc3-a47a-6bedab6470db`:

```
GET https://<instance-name>.westcentralus.azuresmartspaces.net/management/api/v1.0/sensors/c4cf2f41-edd6-4fc3-a47a-6bedab6470db?includes=Types

{
    "dataType":"Motion",
    "dataTypeId":27
    "deviceId":"f51e5295-ca81-4517-8f5d-0b940f678ef2",
    "id": "c4cf2f41-edd6-4fc3-a47a-6bedab6470db",
    "hardwareId":"1234ABC",
    "name": "My Sample Sensor",
    "spaceId": "ef46f69f-0b95-45ee-a41f-718b3ee4c355"
}

```

Copy the `hardwareId` and `dataType` over to be one entry in the `Sensors` array in the [appsettings.json](./appsettings.json) file:

```

{
  "Settings": {
    "Sensors": [{
      "DataType": "Motion",
      "HardwareId": "1234ABC"
    },{
      "DataType": "CarbonDioxide",
      "HardwareId": "SOMEOTHERID"
    }]
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

This app compiles a sample telemetry message with some randomly generated data. You can customize the message format as well as the payload by modifying [models/CustomTelemetryMessage.cs](./models/CustomTelemetryMessage.cs). The data contract of the model can be changed to any serializable format or you can choose to compile your own payload by generating a byte array or stream that can be passed to  [Message(byte[] byteArray)](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.devices.client.message.-ctor?view=azure-dotnet#Microsoft_Azure_Devices_Client_Message__ctor_System_Byte___), as found in the `SendEvent` method. You will have to maintain a set of properties to ensure your message keeps getting routed appropriately, as listed below.

### Telemetry Properties

While the payload contents of a `Message` can be arbitrary data, up to 256kb in size, there are a few requirements on expected [Message.Properties](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.devices.client.message.properties?view=azure-dotnet). The following is the list of required and optional properties supported by the system:

| Property Name | Value | Required | Description |
|---------------|-------|----------|-------------|
| DigitalTwins-Telemetry | 1.0 | yes | A constant value that identifies a message to the system |
| DigitalTwins-SensorHardwareId | `string(72)` | yes | A unique identifier pointing to a Sensor in the topology for which the `Message` is meant for. This value must match an object's `HardwareId` property for the system to process it. E.g.: `00FF0643BE88-PIR` |
| CreationTimeUtc | `string` | no | An [ISO 8601](https://en.wikipedia.org/wiki/ISO_8601) formatted date string identifying the sampling time of the payload. E.g.: `2018-09-20T07:35:00.8587882-07:00` |
| CorrelationId | `string` | no | A `uuid` formatted as a string that can be used to trace events across the system. E.g.: `cec16751-ab27-405d-8fe6-c68e1412ce1f`
