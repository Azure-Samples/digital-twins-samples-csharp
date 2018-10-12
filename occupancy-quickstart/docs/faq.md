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

