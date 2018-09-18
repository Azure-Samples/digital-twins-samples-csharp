# Occupancy Sample

Sample code to provision and read resources in a Digital Twins topology via management APIs. It also creates an example function that runs within the Digital Twins instance that computes motion from a sensor in a room in order to determine the occupancy.

## Building and Running

### Use the terminal

1. `dotnet restore`
2. [Update appsettings.json](#configuring-appsettings.json)
3. `cd src`
4. `dotnet run` to build and see usage
5. `dotnet test ../tests` to build and run tests

### Use VSCode

1. Open the 'occupancy-quickstart' folder in vscode
2. [Update appsettings.json](#configuring-appsettings.json)
3. Run the app by using F5.  You can change the commandline parameters in launch.json
4. To build and run tests use the 'Run Task' command in vscode and choose 'test'

## Configure appsettings.json

//TODO

## Walkthrough

* See [TODO: add docs quickstart link] for how to configure your Digitial Twins resource and configure this app to be able to call it
* Provision spaces, IoT Hub resource, devices, sensors, functions
* Get spaces by parent id
* Send motion telemetry every X seconds to simulate motion sensors
* View occupancy of a spaces of type “Focus Room”

## Notes

### Authentication

//TODO: look into alternate auth (url auth like azure cli uses) and then document the setup for the auth we go with (either here or in docs)