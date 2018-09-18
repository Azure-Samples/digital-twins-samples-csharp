# Digital Twins Device Connectivity Sample

This introductory sample demonstrates how to connect a device to Digital Twins and submit sample sensory data.

## Configure the app

Edit [appsettings.json](./appsettings.json) and supply the following values:

1. `ManagementApiUrl`
2. `SasToken`

>Note: You have already generated a SAS Token from a configured Key on the Keystore API. To generate a valid token, you will need to
know your device's MAC address. Remove all columns (:) from the address and capitalize the value. E.g.: If your MAC address is `18:36:ba:0c:85:13`, the expected value would be `1836BA0C8513`. Please ensure this value is also reflected on the `HardwareId` property of your Topology Device.  If you need to override the hardwareId you can simply edit Program.cs and manually set the hardwareId.

Example configurations:

```json
"ManagementApiUrl": "https://name.westcentralus.azuresmartspaces.net/management/","SasToken": "SharedAccessSignature id=1836BA0C8519&se=31556995200&kv=1&sig=nDBUdQcEXkaRgx3Jy3ntwEJ08uP9KxkjoKR2Wa7lCfs%3D",
```  

## Build the app

1. [Install .NET Core SDK](https://www.microsoft.com/net/core) on your execution platform.
2. Run `dotnet restore`
3. Run `dotnet build`

## Execute the app

* Run `dotnet run`

>Note: Here is how you can run this app on a Raspberry Pi

## Run your app on a Raspberry Pi

### Build the app

Choose `win-arm` or `linux-arm` for your platform:
* Run `dotnet publish -r <platform>`

Under `./bin/Debug/netcoreapp2.0/<runtime identifier>/publish` or `.\bin\Debug\netcoreapp2.0\<runtime identifier>\publish` you will see the whole self contained app that you need to copy to your Raspberry Pi.

### Windows 10 IoT

* Run on device by invoking the executable.

### Linux

1. Install [Linux](https://www.raspberrypi.org/downloads/) on your Pi.
2. Install the [platform dependencies from your distro's package manager](https://github.com/dotnet/core/blob/master/Documentation/prereqs.md) for .NET Core.
3. `chmod 755 ./device-connectivity`
