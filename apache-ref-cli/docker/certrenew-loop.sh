#!/bin/bash

while true; do

if [ -f /lego/certificates/${DOMAIN_NAME}.crt ]; then 
	#renew
	lego --accept-tos --path /lego --http --http.webroot /acme --domains "${DOMAIN_NAME}" --email "${ADMIN_EMAIL}" renew --days 15\
	&& cp -f /lego/certificates/${DOMAIN_NAME}.crt /etc/apache2/certs/server.crt \
	&& cp -f /lego/certificates/${DOMAIN_NAME}.key /etc/apache2/certs/server.key

else
	#issue
	echo first issue
	mkdir /lego
	lego --accept-tos --path /lego --http --http.webroot /acme --domains "${DOMAIN_NAME}" --email "${ADMIN_EMAIL}" run \
	&& cp -f /lego/certificates/${DOMAIN_NAME}.crt /etc/apache2/certs/server.crt \
	&& cp -f /lego/certificates/${DOMAIN_NAME}.key /etc/apache2/certs/server.key \
	&& apache2ctl restart

fi

sleep 3600
done 

