# ultraschall-installer-action

```powershell
choco install wixtoolset
```

```powershell
msbuild -target:restore -property:Configuration=Release -property:RestorePackagesConfig=True
msbuild -target:build -property:Configuration=Release
```
