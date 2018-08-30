# Occupancy Sample

Sample code to provision and read resources in a Digital Twins topology via management apis. It also creates an exmaple function that runs within the Digital Twins which computes motion from a sensor in a room inorder to determine the occupancy.

## Building and Running

### Using terminal
1. `dotnet restore`
1. [Update appsettings.json](#configuring-appsettings.json)
1. `cd src`
1. `dotnet run` to see usage
1. `dotnet test ../tests` to build and run tests

### Using VSCode
1. Open the 'occupancy' folder in vscode
1. [Update appsettings.json](#configuring-appsettings.json)
1. Run the app by using F5.  You can change the commandline parameters in launch.json
1. To build and run tests use the 'Run Task' command in vscode and choose 'test'

## Configuring appsettings.json
TODO

## Walkthrough

* See [TODO: add docs quickstart link] for how to configure your Digitial Twins resource and configure this app to be able to call it
* Provision spaces, IoT Hub resource, devices, sensors, functions
* Get spaces by parent id
* Send motion telemetry every X seconds to simulate motion sensors
* View occupancy of a spaces of type “Focus Room”

## Notes

### Authentication
TODO: look into alternate auth (url auth like azure cli uses) and then document the setup for the auth we go with (either here or in docs)