# Digital Twins Device Connectivity Sample

This sample provides a small framework to connect a device to Digital Twins providing
sample sensory data.

## Configuring the app

Edit [appsettings.json](./appsettings.json) to fill in the following values:
* `ManagementApiUrl`
* `SasToken`

>Note: You have already generated a SAS Token from a configured Key on the Keystore API. To generate a valid token, you will need to
know your device's MAC address. Remove all columns (:) from the address and capitalize the value. E.g.: If your MAC address is `18:36:ba:0c:85:13`, the expected value would be `1836BA0C8513`. Please ensure this value is also reflected on the `HardwareId` property of your Topology Device.  If you need to override the identifier you can simply edit Program.cs and manually set the `hardwareId` variable.

Examples of configuration
```
"ManagementApiUrl": "https://name.westcentralus.azuresmartspaces.net/management/","SasToken": "SharedAccessSignature id=1836BA0C8519&se=31556995200&kv=1&sig=nDBUdQcEXkaRgx3Jy3ntwEJ08uP9KxkjoKR2Wa7lCfs%3D",
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
