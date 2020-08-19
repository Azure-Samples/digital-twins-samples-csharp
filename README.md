---
page_type: sample
languages:
- csharp
products:
- azure
- azure-digital-twins
name: Digital Twins Samples [ARCHIVED]
description: "For the older version of Azure Digital Twins: This repo contains .NET Core samples that demonstrate how to use the Azure Digital Twins platform. Each folder contains a separate .NET Core app."
---

# Digital Twins Samples [ARCHIVED]

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**************************************************
**NOTE (July 2020): A new version of the Azure Digital Twins service has been released, with new features and new implementation details. This sample repository corresponds to the previous set of documentation, which has now been [archived](https://docs.microsoft.com/en-us/previous-versions/azure/digital-twins/about-digital-twins). It no longer reflects the current Azure Digital Twins best practices and is no longer being maintained.**

**To view the latest information for the new service, visit the active [Azure Digital Twins Preview Documentation](https://docs.microsoft.com/en-us/azure/digital-twins/) and its [samples](https://docs.microsoft.com/samples/azure-samples/digital-twins-samples/digital-twins-samples/).**
**************************************************

This repo contains .NET Core samples that demonstrate how to use the Azure Digital Twins platform. Each folder contains a separate .NET Core app.  

See the `README.md` in each sub-folder for specific details about each app:

* [Occupancy sample](https://github.com/Azure-Samples/digital-twins-samples-csharp/tree/master/occupancy-quickstart/README.md)
* [Device Connectivity sample](https://github.com/Azure-Samples/digital-twins-samples-csharp/tree/master/device-connectivity/README.md)

## Get Started

1. [Install dotnet core](https://www.microsoft.com/net/download).
1. Clone the repo:

```shell
git clone https://github.com/Azure-Samples/digital-twins-samples-csharp.git
cd digital-twins-samples-csharp
```

The repo contains several standalone projects:

* The [Occupancy](https://github.com/Azure-Samples/digital-twins-samples-csharp/tree/master/occupancy-quickstart/README.md) sample is suggested as a first example to gain familiarity with Digital Twins.
* The [Device Connectivity](https://github.com/Azure-Samples/digital-twins-samples-csharp/tree/master/device-connectivity/README.md) sample demonstrates how to connect a device to Digital Twins and submit sensory data.

For corresponding documentation, please see the project `README's` above.

## Visual Studio Code

A [workspace file](https://github.com/Azure-Samples/digital-twins-samples-csharp/blob/master/digital-twins-samples.code-workspace) containing all the apps is included for [Visual Studio Code](https://code.visualstudio.com/) users.

Alternatively, each app can be opened individually.

## Licensing and Use

Azure Digital Twins Samples are [MIT Licensed](https://github.com/Azure-Samples/digital-twins-samples-csharp/blob/master/LICENSE.md).

## See also

* Azure Digital Twins [product documentation](https://docs.microsoft.com/azure/digital-twins/)

* Azure Digital Twins [User-defined functions client library reference](https://docs.microsoft.com/azure/digital-twins/reference-user-defined-functions-client-library)
