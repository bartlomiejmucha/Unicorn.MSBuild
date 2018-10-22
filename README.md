# Unicorn.MSBuild

[![NuGet](https://img.shields.io/nuget/v/Unicorn.MSBuild.svg)][1]

The extension adds `SyncUnicorn` target to your project that can be used by MSBuild.

## Installation

Install the [`Unicorn.MSBuild`][1] NuGet package in the web root project.

## Configuration

Add `<ProjectName>.wpp.targets` file to your project's directory with the following content:

```xml
<Project>
    <PropertyGroup>
        <UnicornControlPanelUrl>https://habitat.sc/unicorn.aspx</UnicornControlPanelUrl>
        <UnicornSharedSecret>zUcdjtAKn21fEXIqFnrSzUcdjtAKn21fEXIqFnrSzUcdjtAKn21fEXIqFnrS</UnicornSharedSecret>
    </PropertyGroup>
</Project>
```
For example, if your project file is `Website.csproj`, create `Website.wpp.targets` file in the same directory and put the above content into the file. The `Website.wpp.targets` file will be loaded by `msbuild` automatically.

## Usage

### Visual Studio Extension

The easiest way to execute `SyncUnicorn` target is by installing [`Sync Unicorn`][2] extension. The extension adds _Sync Unicorn_ button to _Build_ menu and to _right click context menu_ of your project.

### Command line

You can also execute `SyncUnicorn` target from command line:

`msbuild Website.csproj /t:SyncUnicorn /p:UnicornControlPanelUrl=https://habitat.sc/unicorn.aspx /p:UnicornSharedSecret=zUcdjtAKn21fEXIqFnrSzUcdjtAKn21fEXIqFnrSzUcdjtAKn21fEXIqFnrS`

`SyncUnicorn` target requires two parameters: `UnicornControlPanelUrl` and `UnicornSharedSecret`. You can pass them directly in command line or use `Website.wpp.targets` file decribed above. 

### Visual Studio external tool

To add a new menu button without installing extension, go to Visual Studio, click Tools and select External Tools.
Add a new external tool with following parameters:

* Title: `Sync Unicorn`
* Command: Path to msbuild.exe for example: `C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe`
* Arguments: The same arguments you add in command line, but in the following example we stored url and secret in publish profile `$(ProjectFileName) /t:SyncUnicorn /p:PublishProfile=FileSystem`
* Initial Directory: `$(ProjectDir)` 

Now you can select your Webroot project in Solution Explorer and go to Tools->Sync Unicorn

## Build

To build nuget package you can use following command:

`nuget.exe pack Unicorn.MSBuild.csproj -properties Configuration=Release -Tool`

[1]: https://www.nuget.org/packages/Unicorn.MSBuild
[2]: https://marketplace.visualstudio.com/items?itemName=BartomiejMucha.SyncUnicorn
