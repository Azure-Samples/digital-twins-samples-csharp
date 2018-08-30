# Occupancy Sample

Sample code to provision and read resources in a Digital Twins topology via management apis. It also creates an exmaple function that runs within the Digital Twins which computes motion from a sensor in a room inorder to determine the occupancy.

## Walkthrough

* See [TODO: add docs quickstart link] for how to configure your Digitial Twins resource and configure this app to be able to call it
* Provision spaces, IoT Hub resource, devices, sensors, functions
* Get spaces by parent id
* Send motion telemetry every X seconds to simulate motion sensors
* View occupancy of a spaces of type “Focus Room”

## Notes

### Authentication
TODO: look into alternate auth (url auth like azure cli uses) and then document the setup for the auth we go with (either here or in docs)