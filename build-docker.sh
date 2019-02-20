#!/bin/bash

BUILDVER=$(date +%F.%s)
DIR=$(realpath $(dirname $0))

git tag -a BUILD_${BUILDVER} -m "automatic build tag"

echo Building ${DIR}/csv-validator/PackageValidation/PackageValidation/PackageValidation.csproj

# build using docker dotnet core image 
docker run --rm -v ${DIR}/csv-validator:/work \
       microsoft/dotnet:2.2.104-sdk \
       dotnet publish \
       -c Release -r linux-x64 \
       /work/PackageValidation/PackageValidation/PackageValidation.csproj

docker run --rm -v ${DIR}/csv-validator:/work \
       microsoft/dotnet:2.2.104-sdk \
       dotnet publish \
       -c Release -r win-x64 \
       /work/PackageValidation/PackageValidation/PackageValidation.csproj

# fix ownership
docker run --rm -v ${DIR}/csv-validator:/work \
       microsoft/dotnet:2.2.104-sdk \
       chown -R $(id -u):$(id -g) /work

mkdir -p ${DIR}/build

cp -R csv-validator/PackageValidation/PackageValidation/bin/Release/netcoreapp2.1/linux-x64/publish ${DIR}/build/csv-validator-linux
cp -R csv-validator/PackageValidation/PackageValidation/bin/Release/netcoreapp2.1/win-x64/publish ${DIR}/build/csv-validator-win


(
	cd ${DIR}/build/
	zip  -r csv-validator-linux.zip  csv-validator-linux
	zip  -r csv-validator-win.zip  csv-validator-win

)

