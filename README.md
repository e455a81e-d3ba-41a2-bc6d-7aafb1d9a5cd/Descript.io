

# Descript.io

![CI Build status](https://descriptio.visualstudio.com/_apis/public/build/definitions/e2d445fe-8893-4fad-9387-10f513d44c02/3/badge)
[![CodeFactor](https://www.codefactor.io/repository/github/lariscusobscurus/descript.io/badge)](https://www.codefactor.io/repository/github/lariscusobscurus/descript.io)

A .NET library for customizable parsing of markdown documents and transforming into other formats.

## Basic structure

Descript.io consists of four projects:

### Descriptio

This project acts as a meta-package which references the other Descript.io packages.
Additionally, it provides an easy-to-use API surface.

### Descriptio.Core

This project contains the structure of the abstract syntax tree.

### Descriptio.Parser

This project contains the markdown lexer and parser components.
It is written entirely in F# and uses many syntactic advantages compared to a parser written in C#.

### Descriptio.Transform

This project contains the formatters for HTML, LaTex, XML and JSON.

## Usage

Using NuGet CLI

```bat
nuget install Descriptio
```

Using dotnet CLI

```bat
dotnet add package Descriptio
```
All published versions can be found on [NuGet](https://www.nuget.org/packages/Descriptio).

## Using the Descript.io library

The easiest way to use the library is by using the integrated Fluent API.

```csharp
using Descriptio;

// generate LaTex out of markdown
var latexResult = Parse.MarkdownString("# Hello World!")
                       .AndFormatToLaTexString();

// generate HTML out of markdown
var htmlResult = Parse.MarkdownString("# Hello World!")
                      .AndFormatToHtmlString();
```

## Contribution

See [CONTRIBUTING](CONTRIBUTING)

## License

This project is licensed using the MIT license.
You can find it in the file [LICENSE](https://raw.githubusercontent.com/LariscusObscurus/Descript.io/master/LICENSE)
