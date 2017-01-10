## Synopsis

[![CSharpToTypeScript](https://github.com/another-guy/CSharpToTypeScript/raw/master/CSharpToTypeScript.png)](https://github.com/another-guy/CSharpToTypeScript)

Translates C# DTOs into compatible TypeScript classes

## Dashboard

[![NuGet](https://img.shields.io/nuget/v/CSharpToTypeScript.svg)](https://www.nuget.org/packages/CSharpToTypeScript/)
[![MyGet CI](https://img.shields.io/myget/another-guy/vpre/CSharpToTypeScript.svg)](https://www.myget.org/feed/another-guy/package/nuget/CSharpToTypeScript) 
[![Build status](https://ci.appveyor.com/api/projects/status/4evhnumtigeukvih?svg=true)](https://ci.appveyor.com/project/another-guy/csharptotypescript) 
[![GitHub issues](https://img.shields.io/github/issues/another-guy/csharptotypescript.svg?maxAge=2592000)](https://github.com/another-guy/CSharpToTypeScript/issues)
[![license](https://img.shields.io/github/license/another-guy/CSharpToTypeScript.svg)](https://github.com/another-guy/CSharpToTypeScript/blob/master/LICENSE)

### Please notice that while this project acknowledges the idea of [Semantic Versioning](http://semver.org/), it is not really followed until `v1.0.0` is released. In other words, as of now *the API is a subject to change without any prior notice*. Stable API is a part of `v1.0.0` requirements, and when it is release, any new releases with breaking changes will affect the major version of the package.

**[Milestones](https://github.com/another-guy/CSharpToTypeScript/milestones?direction=asc&sort=due_date&state=open):**

* <del>[v0.0.1 (alpha)](https://github.com/another-guy/CSharpToTypeScript/milestone/1)</del> **Released**
* [v0.0.2 (alpha)](https://github.com/another-guy/CSharpToTypeScript/milestone/3) **Current**
* [v0.0.3 (alpha)](https://github.com/another-guy/CSharpToTypeScript/milestone/2)
* [v0.0.4 (alpha)](https://github.com/another-guy/CSharpToTypeScript/milestone/5)
* [v1.0.0 (release)](https://github.com/another-guy/CSharpToTypeScript/milestone/4)

## Launch CSharpToTypeScript from console

```
# from <repoRoot>
dotnet restore

# from <repoRoot> execute
dotnet build

# from <repoRoot> execute
dotnet run -p CSharpToTypeScript -- -c CSharpToTypeScript.Tests/SampleFiles/sample.cfg.json
```

## Code Example

// TODO

## Motivation

Manual maintainance of compatible C# data transfer objects (DTO) and their TypeScript counterparts has always been burdensome.
This library is intended to help generating TypeScript enums and classes based on C# type definitions.

## Installation

// TODO

## Tests

Unit tests are available in CSharpToTypeScript.Tests project.

## License

The code is distributed under the MIT license.

## Reporting an Issue

Reporting an issue, proposing a feature, or asking a question are all great ways to improve software quality.

Here are a few important things that package contributors will expect to see in a new born GitHub issue:
* the relevant version of the package;
* the steps to reproduce;
* the expected result;
* the observed result;
* some code samples illustrating current inconveniences and/or proposed improvements.

## Contributing

Contribution is the best way to improve any project!

1. Fork it!
2. Create your feature branch (```git checkout -b my-new-feature```).
3. Commit your changes (```git commit -am 'Added some feature'```)
4. Push to the branch (```git push origin my-new-feature```)
5. Create new Pull Request

...or follow steps described in a nice [fork guide](http://kbroman.org/github_tutorial/pages/fork.html) by Karl Broman
