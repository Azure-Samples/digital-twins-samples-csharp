# Occupancy Sample
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contribute](https://img.shields.io/badge/PR%27s-welcome-brightgreen.svg)](../CONTRIBUTING.md)

Sample code to provision and read resources in a Digital Twins topology via management APIs. It also creates an example function that runs within the Digital Twins instance that computes motion from a sensor in a room in order to determine the occupancy.

## Build and Run the Sample

### Use the terminal

1. Run the `dotnet restore` command.
1. Update `appsettings.json`.
1. Run the `cd src` command.
1. Run the `dotnet run` command to build and see usage.
1. Run the `dotnet test ../tests` command to build and run tests.

### Use Visual Studio Code

1. Open the 'occupancy-quickstart' folder in Visual Studio Code.
1. Update `appsettings.json`.
1. Run the app by using the `F5` key. You can change the command-line parameters in `launch.json`.
1. To build and run tests use the 'Run Task' command in Visual Studio Code and choose `test`.

## Walkthrough

1. See the [quickstart doc](https://docs.microsoft.com/azure/digital-twins/quickstart-view-occupancy-dotnet) to set up your Digital Twins resource and configure this sample to call it.
1. Provision spaces, IoT Hub resource, devices, sensors, functions.
1. Get spaces by parent `id`.
1. Send motion telemetry every X seconds to simulate motion sensors.
1. View occupancy of a spaces of type `Focus Room`.

## Notes

### Authentication

To learn more about configuration, security, and API authentication:

1. See the [role-based access control doc](https://docs.microsoft.com/azure/digital-twins/security-role-based-access-control).
1. See the [Management API doc](https://docs.microsoft.com/azure/digital-twins/security-authenticating-apis).
