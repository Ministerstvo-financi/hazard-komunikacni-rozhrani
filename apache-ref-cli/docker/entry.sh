#!/bin/bash

echo Domain: ${DOMAIN_NAME}
echo Email: ${ADMIN_EMAIL}

if [ ! -f /lego/certificates/${DOMAIN_NAME}.crt ]; then 
	openssl genrsa -des3 -passout pass:xxxxx -out /tmp/server.pass.key 2048
	openssl rsa -passin pass:xxxxx -in /tmp/server.pass.key -out /etc/apache2/certs/server.key
	rm /tmp/server.pass.key
	openssl req -new -key /etc/apache2/certs/server.key -out /tmp/server.csr -subj "/CN=${DOMAIN_NAME}"
	openssl x509 -req -days 3650 -in /tmp/server.csr -signkey /etc/apache2/certs/server.key -out /etc/apache2/certs/server.crt
	rm /tmp/server.csr
else
	cp -f /lego/certificates/${DOMAIN_NAME}.crt /etc/apache2/certs/server.crt \
	&& cp -f /lego/certificates/${DOMAIN_NAME}.key /etc/apache2/certs/server.key 
fi

/certrenew-loop.sh &

echo "chraneny datovy adresar vzdaleneho pristupu" > /var/www/html/data/index.html

#until /usr/sbin/apache2ctl configtest; do
#    echo "==========apache not configured .. sleeping for 5 seconds($(date))"
#    sleep 5
#done

export APACHE_SERVER_NAME=${DOMAIN_NAME}

exec /usr/sbin/apache2ctl -DFOREGROUND