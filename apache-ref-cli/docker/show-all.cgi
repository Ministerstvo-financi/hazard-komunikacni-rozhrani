#!/bin/sh
echo "Content-type: text/plain"
echo
IFS=
echo '%{SSL_CLIENT_S_DN} eq' \"$(echo $SSL_CLIENT_CERT | openssl x509 -subject -noout | sed -e "s/subject=//")\"
echo '%{SSL_CLIENT_I_DN} eq' \"$(echo $SSL_CLIENT_CERT | openssl x509 -issuer -noout | sed -e "s/issuer=//")\"

echo 
echo "===================================================================="
echo 

echo SSL_CLIENT_S_DN:  $SSL_CLIENT_S_DN

echo 
echo "===================================================================="
env