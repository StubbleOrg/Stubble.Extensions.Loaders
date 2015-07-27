# Stubble Extensions - Template Loaders

[![Build status](https://img.shields.io/appveyor/ci/Romanx/stubble-extensions-loaders.svg?style=flat-square)](https://ci.appveyor.com/project/Romanx/stubble-extensions-loaders)
[![Coverage Status](https://img.shields.io/coveralls/StubbleOrg/Stubble.Extensions.Loaders.svg?style=flat-square)](https://coveralls.io/r/StubbleOrg/Stubble.Extensions.Loaders)
[![Prerelease Nuget](https://img.shields.io/nuget/vpre/Stubble.Extensions.Loaders.svg?style=flat-square&label=nuget%20pre)](https://www.nuget.org/packages/Stubble.Extensions.Loaders/)
[![Stable Nuget](https://img.shields.io/nuget/v/Stubble.Extensions.Loaders.svg?style=flat-square)](https://www.nuget.org/packages/Stubble.Extensions.Loaders/)

The loaders provided in this repository extend Stubble's very basic template loading features with some that cover common scenarios.

All of the loaders implement the IStubbleLoader interface and so can be composited together in a cascading manner using the CompositeLoader provided by default with Stubble but can also be used standalone.

## Array Loader
The array loader is the most basic of the set, on instantiation you provide a dictionary of templates which are looked up on load. The most basic usage is:

```csharp
var templates = new Dictionary<string, string> {
	{ "MyTemplate", "{{foo}}" }
};

var loader = new ArrayLoader(templates);
```
*Note: The dictionary passed into the loader is copied and so it cannot be changed after being passed into the loader.*

## Filesystem Loader
The file system loader takes a path and optionally a file extension on instantiation and will load a template from a file matching the requested name.

The file extension can also be passed into the constructor but by default is `.mustache`.

```csharp
var loader = new FileSystemLoader("./templates")
var secondLoader = new FileSystemLoader("./templates", "mst")
```

Since reading from the file system can be a slow process the loaded file is cached in the loader so to avoid disk reads.

## Embedded Resources Loader
C# provides a method of embedding resources into the DLL causing the size to inflate but making distribution of the DLL easier. This loader allows these resources to be discovered and loaded.

Since the resources are embedded inside a DLL they have to be loaded from an Assembly. By default the loader will assign the calling assembly however the assembly can be passed into the constructor. The file extension can also be passed into the constructor but by default is `.mustache`.

```csharp
var loader = new EmbeddedResourceLoader()
var loaderWithExtension = new EmbeddedResourceLoader("mst")
var loaderWithAssembly = new EmbeddedResourceLoader(Assembly.Load("AssemblyName"))
var loaderWithAssemblyAndExtension = new EmbeddedResourceLoader(Assembly.Load("AssemblyName"), "mst")
```
