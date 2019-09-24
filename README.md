---
page_type: sample
languages:
- csharp
products:
- azure
- azure-digital-twins
description: "This repo contains .NET Core samples that demonstrate how to use the Azure Digital Twins platform. Each folder contains a separate .NET Core app."
---

# Digital Twins Samples

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contribute](https://img.shields.io/badge/PR%27s-welcome-brightgreen.svg)](CONTRIBUTING.md)

This repo contains .NET Core samples that demonstrate how to use the Azure Digital Twins platform. Each folder contains a separate .NET Core app.  

See the `README.md` in each sub-folder for specific details about each app:

* [Occupancy sample](./occupancy-quickstart/README.md)
* [Device Connectivity sample](./device-connectivity/README.md)

## Get Started

1. [Install dotnet core](https://www.microsoft.com/net/download).
1. Clone the repo:

```shell
git clone https://github.com/Azure-Samples/digital-twins-samples-csharp.git
cd digital-twins-samples-csharp
```

The repo contains several standalone projects:

* The [Occupancy](./occupancy-quickstart/README.md) sample is suggested as a first example to gain familiarity with Digital Twins.
* The [Device Connectivity](./device-connectivity/README.md) sample demonstrates how to connect a device to Digital Twins and submit sensory data.

For corresponding documentation, please see the project `README's` above.

## Visual Studio Code

A [workspace file](./digital-twins-samples.code-workspace) containing all the apps is included for [Visual Studio Code](https://code.visualstudio.com/) users.

Alternatively, each app can be opened individually.

## Licensing and Use

Azure Digital Twins Samples are [MIT Licensed](./LICENSE.md).
