# Digital Twins Device Connectivity Sample

This introductory sample demonstrates how to connect a device to Digital Twins and submit sample sensory data.

## Configure the app

Edit [appsettings.json](./appsettings.json) and supply the following values:

1. `ManagementApiUrl`
2. `SasToken`

//TODO Add the SAS Token
> Note: You must have already generated a SAS Token using the [Key Store]() API.

To generate a valid token:

1. You will need to know your device's MAC address.
2. Remove all columns (:) from the address and capitalize the value. If your MAC address is `18:36:ba:0c:85:13`, the expected value would be `1836BA0C8513`.
3. Please ensure this value corresponds to the `hardwareId` property of your Topology Device.  To override the `hardwareId` you can simply edit `Program.cs` and manually set the `hardwareId`.

Example configurations:

```
"ManagementApiUrl": "https://name.westcentralus.azuresmartspaces.net/management/",
"SasToken": "SharedAccessSignature id=1836BA0C8519&se=31556995200&kv=1&sig=nDBUdQkaRgx3Jy3n..."
```  

## Build the app

1. [Install .NET Core SDK](https://www.microsoft.com/net/core) on your execution platform.
2. Run the `dotnet restore` command.
3. Run the `dotnet build` command.

## Execute the app

* Run the `dotnet run` command.

>Note: Here is how you can run this app on a Raspberry Pi.

## Run your app on a Raspberry Pi

### Build the app

Choose `win-arm` or `linux-arm` for your platform:
* Run the `dotnet publish -r <platform>` command.

Under `./bin/Debug/netcoreapp2.0/<runtime identifier>/publish` or `.\bin\Debug\netcoreapp2.0\<runtime identifier>\publish` you will see the whole self contained app that you need to copy to your Raspberry Pi.

### Windows 10 IoT

* Run on device by invoking the executable.

### Linux

1. Install [Linux](https://www.raspberrypi.org/downloads/) on your Pi.
2. Install the [platform dependencies](https://github.com/dotnet/core/blob/master/Documentation/prereqs.md) for .NET Core through your distribution's package manager.
3. Run the `chmod 755 ./device-connectivity` command.
