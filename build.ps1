Function Remove-BuildArtifacts {
  Remove-Item -Recurse -Force -ErrorAction SilentlyContinue -Path .\bin
  Remove-Item -Recurse -Force -ErrorAction SilentlyContinue -Path .\obj
  Remove-Item -Recurse -Force -ErrorAction SilentlyContinue -Path .\packages
}

Remove-BuildArtifacts

msbuild -target:restore -property:Configuration=Release -property:TargetFrameworkVersion=4.7.1 -property:RestorePackagesConfig=True
msbuild -target:build -property:Configuration=Release -property:TargetFrameworkVersion=4.7.1
