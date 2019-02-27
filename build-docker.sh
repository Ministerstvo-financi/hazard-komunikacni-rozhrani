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
	if [ -z "$BUILD_SKIP_LINUX" ]; then
	docker run --rm -v ${DIR}/csv-validator:/work \
	       microsoft/dotnet:2.2.104-sdk \
	       dotnet publish \
	       -c Release -r linux-x64 --version-suffix ${VERSUFF} \
	       /work/PackageValidation/PackageValidation/PackageValidation.csproj
	else 
		echo "############## LINUX BUILD SKIPPED #################"
	fi

	#windows build
	if [ -z "$BUILD_SKIP_WIN" ]; then
	docker run --rm -v ${DIR}/csv-validator:/work \
	       microsoft/dotnet:2.2.104-sdk \
	       dotnet publish \
	       -c Release -r win-x64 --version-suffix ${VERSUFF} \
	       /work/PackageValidation/PackageValidation/PackageValidation.csproj
	else 
		echo "############## WIN BUILD SKIPPED #################"
	fi


	#macosx build
	if [ -z "$BUILD_SKIP_MAC" ]; then
	docker run --rm -v ${DIR}/csv-validator:/work \
	       microsoft/dotnet:2.2.104-sdk \
	       dotnet publish \
	       -c Release -r osx-x64 --version-suffix ${VERSUFF} \
	       /work/PackageValidation/PackageValidation/PackageValidation.csproj
	else 
		echo "############## MAC BUILD SKIPPED #################"
	fi

	# fix ownership
	docker run --rm -v ${DIR}/csv-validator:/work \
	       microsoft/dotnet:2.2.104-sdk \
	       chown -R $(id -u):$(id -g) /work
	       


}

function buildCryptoUtil {
	echo "############################################################################################"
	echo  Building ${DIR}/crypto_utils
	echo "############################################################################################"

	mkdir -p ${DIR}/build/repo 

	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_utils:/work -w /work maven:3.6.0-jdk-11-slim /work/build-linux.sh
	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_cli:/work -w /work maven:3.6.0-jdk-11-slim mvn -DskipTests package

	# fix ownership
	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_utils:/work -w /work maven:3.6.0-jdk-11-slim chown -R $(id -u):$(id -g) /work
	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_cli:/work -w /work maven:3.6.0-jdk-11-slim chown -R $(id -u):$(id -g) /work
	docker run -it --rm  -v ${DIR}/build/repo:/root/.m2 -v ${DIR}/crypto_utils:/work -w /work maven:3.6.0-jdk-11-slim chown -R $(id -u):$(id -g) /root/.m2 

}

function packAll {

	#build csv validator

	rm -rf ${DIR}/build/dist
	mkdir -p ${DIR}/build/dist

	if [ -z "$BUILD_SKIP_LINUX" ]; then
		cp -R csv-validator/PackageValidation/PackageValidation/bin/Release/netcoreapp2.1/linux-x64/publish ${DIR}/build/dist/csv-validator-linux
	fi

    if [ -z "$BUILD_SKIP_WIN" ]; then
		cp -R csv-validator/PackageValidation/PackageValidation/bin/Release/netcoreapp2.1/win-x64/publish ${DIR}/build/dist/csv-validator-win
	fi

	if [ -z "$BUILD_SKIP_MAC" ]; then
		cp -R csv-validator/PackageValidation/PackageValidation/bin/Release/netcoreapp2.1/osx-x64/publish ${DIR}/build/dist/csv-validator-macosx
	fi

	# build crypto utils

	mkdir -p ${DIR}/build/dist/crypto_utils 

	rm -f ${DIR}/build/crypto_utils/*
	cp ${DIR}/crypto_cli/target/crypto_cli-1.0-jar-with-dependencies.jar \
	   ${DIR}/crypto_cli/target/crypto_cli-1.0.jar \
	   ${DIR}/crypto_cli/*.bat \
	   ${DIR}/crypto_utils/target/crypto_utils-1.0-jar-with-dependencies.jar \
	   ${DIR}/crypto_utils/target/crypto_utils-1.0.jar \
	   ${DIR}/build/dist/crypto_utils 

	cp -R ${DIR}/data  \
	    ${DIR}/openssl1.1.0 \
	    ${DIR}/build/dist/ 

	HAZARD=hazard

	echo  "BUILDVER=${BUILDVER}" > ${DIR}/build/dist/version
	echo  "GITID=${GITID}" >> ${DIR}/build/dist/version
	mv ${DIR}/build/dist "${DIR}/build/${HAZARD}-${BUILDVER}"
	(
		cd "${DIR}/build/"
		zip -r "${HAZARD}-${BUILDVER}.zip" "${HAZARD}-${BUILDVER}" 
	)
}

buildCryptoUtil
buildCsvValidator
packAll