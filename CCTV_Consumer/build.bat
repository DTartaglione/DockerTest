msbuild CCTV_Consumer.sln /p:Configuration=Release /p:Platform="Any CPU" -t:restore,build -p:RestorePackagesConfig=true