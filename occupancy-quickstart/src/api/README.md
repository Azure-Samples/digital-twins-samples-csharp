# API

This folder contains helpers that wrap Digital Twins REST API to produce and consume in memory [models](../models/readme.md).  

Additional helpers are supplied to further simplify API interactions.

> Please note that these API helpers are not complete.  

## Swagger

See the Swagger documentation that is part of your Azure Digital Twins instance for a complete description of API capabilities:

>For your reference, a Swagger sneak preview is provided to demonstrate the API feature set.
>It's hosted at [docs.westcentralus.azuresmartspaces.net/management/swagger](https://docs.westcentralus.azuresmartspaces.net/management/swagger).

You can access your own, generated, Management API Swagger documentation at:

```plaintext
https://yourInstanceName.yourLocation.azuresmartspaces.net/management/swagger
```

| Custom Attribute Name | Replace With |
| --- | --- |
| `yourInstanceName` | The name of your Azure Digital Twins instance |
| `yourLocation` | Which server region your instance is hosted on |

## AutoRest

Also see [AutoRest](https://github.com/Azure/autorest) for how you can generate full C# models and APIs from that documentation.
