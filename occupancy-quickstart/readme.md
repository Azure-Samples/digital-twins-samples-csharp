# Occupancy Sample

Sample code to provision and read resources in a Digital Twins topology via management APIs. It also creates an example function that runs within the Digital Twins instance that computes motion from a sensor in a room in order to determine the occupancy.

## Build and Run the Sample

### Use the terminal

1. Run the `dotnet restore` command.
2. [Update appsettings.json](#configuring-appsettings.json).
3. Run the `cd src` command.
4. Run the `dotnet run` command to build and see usage.
5. Run the `dotnet test ../tests` command to build and run tests.

### Use VSCode

1. Open the 'occupancy-quickstart' folder in Visual Studio Code.
2. [Update appsettings.json](#configuring-appsettings.json).
3. Run the app by using the `F5` key. You can change the command-line parameters in `launch.json`.
4. To build and run tests use the 'Run Task' command in Visual Studio Code and choose `test`.

## Configure appsettings.json

//TODO

## Walkthrough

1. See the [quickstart doc](https://github.com/MicrosoftDocs/azure-docs-pr/blob/release-preview-smart-spaces/articles/digital-twins/quickstart-view-occupancy-dotnet.md) to set up your Digital Twins resource and configure this sample to call it.
2. Provision spaces, IoT Hub resource, devices, sensors, functions.
3. Get spaces by parent `id`.
4. Send motion telemetry every X seconds to simulate motion sensors.
5. View occupancy of a spaces of type `Focus Room`.

## Notes

### Authentication

//TODO: look into alternate auth (url auth like azure cli uses) and then document the setup for the auth we go with (either here or in docs)