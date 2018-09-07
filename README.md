# Unicorn.MSBuild
The extension adds `SyncUnicorn` target for your project that can be used by MSBuild.

## Installation

Install the [`Unicorn.MSBuild`][1] NuGet package in the web root project.

## Usage

### Command line

The easiest way is to execute it manually from command line:

`msbuild Website.csproj /t:SyncUnicorn /p:UnicornControlPanelUrl=https://habitat.sc/unicorn.aspx /p:UnicornSharedSecret=zUcdjtAKn21fEXIqFnrSzUcdjtAKn21fEXIqFnrSzUcdjtAKn21fEXIqFnrS`

Of course you can store `UnicornControlPanelUrl` and `UnicornSharedSecret` in your project file, publish profile or another targets file.
You can also integrate it into your build pipeline.

### Visual Studio menu button

To add a new menu button, go to Visual Studio, click Tools and select External Tools.
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
