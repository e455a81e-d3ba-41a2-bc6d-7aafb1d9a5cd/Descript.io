# Descript.io

![CI Build status](https://descriptio.visualstudio.com/_apis/public/build/definitions/e2d445fe-8893-4fad-9387-10f513d44c02/1/badge)
[![CodeFactor](https://www.codefactor.io/repository/github/lariscusobscurus/descript.io/badge)](https://www.codefactor.io/repository/github/lariscusobscurus/descript.io)

A .NET library for customizable parsing of markdown documents and transforming into other formats.

## Basic structure

Descript.io is consists of four projects:

### Descriptio.Core

This package contains the structure of the abstract syntax tree.

### Descriptio.Parser

This package contains the markdown lexer and parser components.
It is written entirely in F# and uses many syntactic advantages compared to a parser written in C#.

### Descriptio.Transform

This package contains the formatters for HTML, LaTex, XML and JSON.

## Usage

Using NuGet CLI

```bat
nuget install Descriptio
```

Using dotnet CLI

```bat
dotnet add package Descriptio
```

## Using the Descript.io library

The easiest way to use the library is by using the integrated Fluent API.

```csharp
using Descriptio;

// generate LaTex out of markdown
string latexResult = Parse.MarkdownString("# Hello World!")
                          .AndFormatToLaTexString();

// generate HTML out of markdown
string htmlResult = Parse.MarkdownString("# Hello World!")
                         .AndFormatToHtmlString();
```

## Contribution

See [CONTRIBUTION](https://raw.githubusercontent.com/LariscusObscurus/Descript.io/master/CONTRIBUTION)

## License

This project is licensed using the MIT license.
You can find it in the file [LICENSE](https://raw.githubusercontent.com/LariscusObscurus/Descript.io/master/LICENSE)
