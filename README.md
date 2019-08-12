# NetRPG

An RPGLE runtime for .NET Core. With this runtime you can run totally free-format RPG code on any system that can run .NET Core.

See our [Trello board](https://trello.com/b/JAbqwxO6/netrpg) for progress.

## Setup

1. Clone repo
2. Open folder in vscode
3. Execute project to run unit tests.

### Executing programs

* Change `launch.json` in vscode to pass in a parameter to `NetRPG.dll`: `"args": ["RPGCode/dcl_file_exfmt9.rpgle"]`
* Launch `NetRPG.dll` from the command line:

```
$ alias netrpg="dotnet NetRPG/bin/Debug/netcoreapp2.0/NetRPG.dll"
$ netrpg RPGCode/op_dsply.rpgle
Hello world
```
