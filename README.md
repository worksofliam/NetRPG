# NetRPG

An RPGLE runtime for .NET Core. With this runtime you can run totally free-format RPG code on any system that can run .NET Core.

[See wiki for more information](https://github.com/WorksOfBarry/NetRPG/wiki).

## :fire: Features

* **Fully free-format RPG only**: write all the latest RPG code in fully free-format.

* **Data-types and complex structures**: A majority of the data-types are supported and complex data-structures work too!

* **Full procedure support**: Use procedures with parameters by ref, const or value. It all works!

* **Full ILE support**: Want to call a procedure from another module? No problem.

* **Display file support**: While not everything is supported, we have the base for display file support.

## :floppy_disk: Installation

1. Clone repo
2. Open folder in vscode
3. Execute `dotnet restore` to fetch the deps.
3. Execute project to run unit tests.

## :computer: Usage

* Launch `NetRPG.dll` from the command line:

```
$ alias netrpg="dotnet bin/Debug/netcoreapp2.0/NetRPG.dll"
$ netrpg RPGCode/op_dsply.rpgle
Hello world
```

* Change `launch.json` in vscode to pass in a parameter to `NetRPG.dll`: `"args": ["RPGCode/dcl_file_exfmt9.rpgle"]`
