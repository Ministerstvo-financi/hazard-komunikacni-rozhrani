#!/bin/sh

DIR=$(dirname $0)
(
cd $DIR
TMPDIR=$(mktemp -d)
mkdir $TMPDIR/cacheTSL

export AISG_openSSL=openssl
export AISG_KeyStoreCertificatePath=src/test/resources/keystore.p12
export AISG_KeyStoreCertificateType=PKCS12
export AISG_KeyStoreCertificatePassword=dss-password
export AISG_CacheTSL=$TEMP/cacheTSL


mvn  -DskipTests clean install

rm -rf $TMPDIR 
)