## Purpose
<!-- Describe the intention of the changes being proposed. What problem does it solve or functionality does it add? -->
* ...

## Pull Request Type
What kind of change does this Pull Request introduce?

<!-- Please check the one that applies to this PR using "x". -->
```
[ ] Bugfix
[ ] New sample or new feature within a sample
[ ] Code style update (formatting, naming)
[ ] Refactoring (no functional changes)
[ ] Documentation content changes
[ ] Other... Please describe:
```

## How to Test
Get the code
```
git clone https://github.com/Azure-Samples/digital-twins-samples-csharp.git
cd digital-twins-samples-csharp
```

Run the tests
```
dotnet restore <folder>
dotnet build <folder>
dotnet restore <folder>.tests
dotnet test <folder>.tests
```

Also verify anything you think is relevent by manually running the sample(s)
```
cd <folder>
dotnet run
```

## Other Information
<!-- Add any other helpful information that may be needed here. -->