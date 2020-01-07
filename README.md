# GitDir

This tool executes a git command for every repository under the path where it is executed.

**Requirements**
- [Git](https://git-scm.com/download)
- [Microsoft .NET Core 3.0](https://dotnet.microsoft.com/download)

**Windows Install**

```
> dotnet run GitDir.csproj
```

**Linux Install**

For the publish runtime flag (-r) choose your system architecture [RID] acording to [Microsoft .NET Core RID Catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#linux-rids)

```
$ dotnet publish -c Release -r [RID] GitDir.csproj
$ cp ./bin/Release/netcoreapp3.0/linux-arm/publish ~/.gitdir
```

**Usage**

Change directory base to where you have your repos and then execute the desired command:

Windows
```
> gitdir [any Git command]
```

Linux
```
> ~/.gitdir/GitDir [any Git command]
```
