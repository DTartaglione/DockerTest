msbuild Weather_DI.sln /p:Configuration=Release /p:Platform="Any CPU" -t:restore,build -p:RestorePackagesConfig=true