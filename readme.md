# Desktop app with database of users, questions and answers

This app will help you to find and add questions and answers. 

## Installation

> Use the .NET framework to install Microsoft.Data.Sqlite, Terminal.Gui and ScottPlot.

Install this in ClassLibrary, ConsoleGenerator and ConsoleServer
```bash
dotnet add package Microsoft.Data.Sqlite
```

Install this in ConsoleApp
```bash
dotnet add package Microsoft.Data.Sqlite
dotnet add package Terminal.Gui
dotnet add package ScottPlot
```

> Also add reference to ClassLibrary in yours programs. For example:
```bash
dotnet add .\ConsoleApp\ConsoleApp.csproj reference .\ClassLibrary\ClassLibrary.csproj
```

## Usage

First of all, start ConsoleServer with
```bash
dotnet run
```
After, start ConsoleApp with
```bash
dotnet run
```

## Aurhor
Anastasia Grabovska

## GiHub repository
[GitHub](https://github.com/grabelka/progbase3)