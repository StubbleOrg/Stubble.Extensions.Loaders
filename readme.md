# Stubble Extensions - Template Loaders [![Build status](https://img.shields.io/appveyor/ci/Romanx/stubble-extensions-loaders.svg?style=flat-square)](https://ci.appveyor.com/project/Romanx/stubble-extensions-loaders) [![Build Status](https://travis-ci.org/StubbleOrg/Stubble.Extensions.Loaders.svg?branch=master)](https://travis-ci.org/StubbleOrg/Stubble.Extensions.Loaders) [![codecov](https://codecov.io/gh/StubbleOrg/Stubble.Extensions.Loaders/branch/master/graph/badge.svg)](https://codecov.io/gh/StubbleOrg/Stubble.Extensions.Loaders) [![Prerelease Nuget](https://img.shields.io/nuget/vpre/Stubble.Extensions.Loaders.svg?style=flat-square&label=nuget%20pre)](https://www.nuget.org/packages/Stubble.Extensions.Loaders/) [![Stable Nuget](https://img.shields.io/nuget/v/Stubble.Extensions.Loaders.svg?style=flat-square)](https://www.nuget.org/packages/Stubble.Extensions.Loaders/)

<img align="right" width="160px" height="160px" src="https://raw.githubusercontent.com/StubbleOrg/Stubble/dev/assets/extension-logo-256.png">

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

Different levels by default are delimited by colons `:` however this can be changed to any delimiter.
These are replaced when looking up the file by the default platform delimiter.
This is `/` on windows and `\` on UNIX.

Since reading from the file system can be a slow process the loaded file is cached in the loader so to avoid disk reads.

## Embedded Resources Loader
C# provides a method of embedding resources into the DLL causing the size to inflate but making distribution of the DLL easier. This loader allows these resources to be discovered and loaded.

Since the resources are embedded inside a DLL they have to be loaded from an Assembly. By default the loader will assign the calling assembly however the assembly can be passed into the constructor. The file extension can also be passed into the constructor but by default is `.mustache`.

```csharp
var loaderWithAssembly = new EmbeddedResourceLoader(Assembly.Load("AssemblyName"))
var loaderWithAssemblyAndExtension = new EmbeddedResourceLoader(Assembly.Load("AssemblyName"), "mst")
```

## Credits

Straight Razor by Vectors Market from the Noun Project