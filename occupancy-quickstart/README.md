# Occupancy Sample

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT) [![Contribute](https://img.shields.io/badge/PR%27s-welcome-brightgreen.svg)](../CONTRIBUTING.md)

Sample code to provision and read resources in a Digital Twins topology via management APIs. It also creates an example function that runs within the Digital Twins instance that computes motion from a sensor in a room in order to determine the occupancy.

## Build and Run the Sample

Below are some details on how to get up and running.  For a more detailed walkthrough or more details on how to get the values described below, please see this [quickstart doc](https://docs.microsoft.com/azure/digital-twins/quickstart-view-occupancy-dotnet).

### Update appSettings.json

`appSettings.json` is used to specify info on which Digital Twins instance to connect to. The three fields you will need to fill in are:

- `ClientId`: The **application ID** of a native Azure Active Directory app that has permissions to call the Azure Digital Twins service.
- `Tenant`: The **directory ID** of a your Azure Active Directory.
- `BaseUrl`: The management api url to your Digital Twins instance (see `appSetting.json` for what this should look like).

### Use a shell

1. Run the app:

    ```shell
    cd src
    dotnet restore
    dotnet run
    ```
    This will show usage.  For a walkthrough of what you can do see [quickstart doc](https://docs.microsoft.com/azure/digital-twins/quickstart-view-occupancy-dotnet).

1. Run tests:

    ```shell
    dotnet test ../tests
    ```

### Use Visual Studio Code

1. Open the 'occupancy-quickstart' folder in Visual Studio Code.
1. Run the app by using the `F5` key. You can change the command-line parameters in `launch.json`.
1. To build and run tests use the 'Run Task' command in Visual Studio Code and choose `test`.

## Notes

### Authentication

To learn more about configuration, security, and API authentication:

1. See the [role-based access control doc](https://docs.microsoft.com/azure/digital-twins/security-role-based-access-control).
1. See the [Management API doc](https://docs.microsoft.com/azure/digital-twins/security-authenticating-apis).

## Problems

If you run into problems or have questions checkout our [FAQ](./docs/faq.md).  If that doesn't help search open and closed [issues](https://github.com/Azure-Samples/digital-twins-samples-csharp/issues) and open a new one if you can't find an answer.

## Licensing and Use

Azure Digital Twins Samples are [MIT Licensed](./LICENSE.md).
