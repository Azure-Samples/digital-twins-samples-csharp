## Occupany Sample FAQ

#### Exception: YamlDotNet.Core.SyntaxErrorException

```
Exception: YamlDotNet.Core.SyntaxErrorException: (Line: X, Col: Y, Idx: ZZ) - (Line: X, Col: Y, Idx: ZZ): While scanning for the next token, find character that cannot start any token.
```

Yaml files don't support tab characters.  NOTE: that visual studio 2017 doesn't know (without customizations) to insert spaces instead of tabs into yaml files.  More details [here](https://developercommunity.visualstudio.com/content/problem/71238/editor-inserts-tabs-instead-of-spaces-for-yaml-fil.html)


#### Response Status: 404, NotFound

```
trce: DigitalTwinsQuickstart[0]
      Request: GET https://yourResource.yourLocation.azuresmartspaces.net/management/somethingWrong
trce: DigitalTwinsQuickstart[0]
      Response Status: 404, NotFound,
```

A 404 usually indicates the `BaseUrl` in appSettings.json (or appSettings.dev.json) is misconfigured.


#### Response Status: 401, Unauthorized

There are several conditions that can cause a 401.  Below are different errors that can be included as part of a 401 and possible corrections.

##### The request body must contain the following parameter: 'client_secret or client_assertion
```
trce: DigitalTwinsQuickstart[0]
      Response Status: 401, Unauthorized, {"message":"Authorization has been denied for this request."}
To sign in, use a web browser to open the page https://microsoft.com/devicelogin and enter the code ZZZZZZZ to authenticate.
Exception: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalServiceException: AADSTS70002: The request body must contain the following parameter: 'client_secret or client_assertion'.
```

This usually indicates the aad app you are using (as represented by `ClientId` in appSettings.json or appSettings.dev.json) 
is an aad *web* app instead of an aad *native* app).


#### Response Status: 403, Forbidden

This usually is because the user account used for login (when prompted by this app) is not authenticated in the Digital Twins' Management Api.  See [role-based access control](https://docs.microsoft.com/en-us/azure/digital-twins/security-role-based-access-control) for more info.