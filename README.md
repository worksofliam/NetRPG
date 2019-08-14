# NetRPG

An RPGLE runtime for .NET Core. With this runtime you can run totally free-format RPG code on any system that can run .NET Core.

See our [Trello board](https://trello.com/b/JAbqwxO6/netrpg) for progress.

## Dependancies

1. [Newtonsoft.Json](https://www.newtonsoft.com/json) is currently used for static RLA.
2. [Terminal.Gui](https://github.com/migueldeicaza/gui.cs) is used for the 5250 emulator.

## Setup

1. Clone repo
2. Open folder in vscode
3. Execute `dotnet restore` to fetch the deps.
3. Execute project to run unit tests.

### Executing programs

* Launch `NetRPG.dll` from the command line:

```
$ alias netrpg="dotnet bin/Debug/netcoreapp2.0/NetRPG.dll"
$ netrpg RPGCode/op_dsply.rpgle
Hello world
```

* Change `launch.json` in vscode to pass in a parameter to `NetRPG.dll`: `"args": ["RPGCode/dcl_file_exfmt9.rpgle"]`