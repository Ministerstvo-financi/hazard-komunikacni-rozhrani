#!/bin/bash

VERSUFF=$(date +%s)
BUILDVER=$(date +%F.%s)
DIR=$(realpath $(dirname $0))
GITID=$(git rev-list --max-count=1 HEAD)


git tag -a BUILD_${BUILDVER} -m "automatic build tag"

function buildCsvValidator {
	echo "############################################################################################"
	echo  Building ${DIR}/csv-validator/PackageValidation/PackageValidation/PackageValidation.csproj
	echo "############################################################################################"

	# build using docker dotnet core image
	# linux build 
	docker run --rm -v ${DIR}/csv-validator:/work \
	       microsoft/dotnet:2.2.104-sdk \
	       dotnet publish \
	       -c Release -r linux-x64 --version-suffix ${VERSUFF} \
	       /work/PackageValidation/PackageValidation/PackageValidation.csproj

	#windows build
	docker run --rm -v ${DIR}/csv-validator:/work \
	       microsoft/dotnet:2.2.104-sdk \
	       dotnet publish \
	       -c Release -r win-x64 --version-suffix ${VERSUFF} \
	       /work/PackageValidation/PackageValidation/PackageValidation.csproj

	#macosx build
	docker run --rm -v ${DIR}/csv-validator:/work \
	       microsoft/dotnet:2.2.104-sdk \
	       dotnet publish \
	       -c Release -r osx-x64 --version-suffix ${VERSUFF} \
	       /work/PackageValidation/PackageValidation/PackageValidation.csproj


	# fix ownership
	docker run --rm -v ${DIR}/csv-validator:/work \
	       microsoft/dotnet:2.2.104-sdk \
	       chown -R $(id -u):$(id -g) /work
	       

	mkdir -p ${DIR}/build
	#rm -rf ${DIR}/build/*

	cp -R csv-validator/PackageValidation/PackageValidation/bin/Release/netcoreapp2.1/linux-x64/publish ${DIR}/build/csv-validator-linux
	cp -R csv-validator/PackageValidation/PackageValidation/bin/Release/netcoreapp2.1/win-x64/publish ${DIR}/build/csv-validator-win
	cp -R csv-validator/PackageValidation/PackageValidation/bin/Release/netcoreapp2.1/osx-x64/publish ${DIR}/build/csv-validator-macosx


	(
		cd ${DIR}/build/
		zip  -r csv-validator-linux.zip  csv-validator-linux
		zip  -r csv-validator-win.zip  csv-validator-win
		zip  -r csv-validator-macosx.zip  csv-validator-macosx
	)

}

function buildCryptoUtil {
	echo "############################################################################################"
	echo  Building ${DIR}/crypto_utils
	echo "############################################################################################"

	mkdir -p ${DIR}/build/repo 

	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_utils:/work -w /work maven:3.6.0-jdk-11-slim /work/build-linux.sh
	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_cli:/work -w /work maven:3.6.0-jdk-11-slim mvn clean install

	# fix ownership
	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_utils:/work -w /work maven:3.6.0-jdk-11-slim chown -R $(id -u):$(id -g) /work
	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_cli:/work -w /work maven:3.6.0-jdk-11-slim chown -R $(id -u):$(id -g) /work
	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_utils:/work -w /work maven:3.6.0-jdk-11-slim chown -R $(id -u):$(id -g) /root/.m2 

	mkdir -p ${DIR}/build/crypto_utils 

	cp ${DIR}/crypto_cli/target/crypto_cli-1.0-jar-with-dependencies.jar \
	   ${DIR}/crypto_cli/target/crypto_cli-1.0.jar \
	   ${DIR}/crypto_cli/*.bat \
	   ${DIR}/crypto_utils/target/crypto_utils-1.0-jar-with-dependencies.jar \
	   ${DIR}/crypto_utils/target/crypto_utils-1.0.jar \
	   ${DIR}/build/crypto_utils 
	(
		cd ${DIR}/build/
		zip  -r crypto_utils.zip crypto_utils
	)
}

rm -rf {DIR}/build
buildCryptoUtil
buildCsvValidator