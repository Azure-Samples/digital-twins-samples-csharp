## Occupany Sample FAQ

#### Exception: YamlDotNet.Core.SyntaxErrorException

```
Exception: YamlDotNet.Core.SyntaxErrorException: (Line: X, Col: Y, Idx: ZZ) - (Line: X, Col: Y, Idx: ZZ): While scanning for the next token, find character that cannot start any token.
```

Yaml files don't support tab characters.  NOTE: that visual studio 2017 doesn't know (without customizations) to insert spaces instead of tabs into yaml files.  More details [here](https://developercommunity.visualstudio.com/content/problem/71238/editor-inserts-tabs-instead-of-spaces-for-yaml-fil.html)