FROM ubuntu:18.04
ENV DEBIAN_FRONTEND=noninteractive
RUN sed -i -e 's!/archive!/cz.archive!' /etc/apt/sources.list \
 && apt update \
 && apt upgrade -y \
 && ln -sf /usr/share/zoneinfo/Europe/Prague /etc/localtime \
 && apt install -y tzdata wget software-properties-common apache2 lego\
 && a2enmod ssl \
 && a2enmod cgi \
 && a2dismod -f autoindex \
 && a2dissite 000-default \
 && a2dissite default-ssl \
 && echo "Hazard referencni klient."> /var/www/html/index.html


RUN mkdir /acme \
 && mkdir /lego \
 && mkdir /var/www/html/data \
 && mkdir /etc/apache2/certs \
 && mkdir /var/www/debug-certificate \
 && echo "plain http server pro vydani a obnovu certifikatu" > /acme/index.html 

ENV APACHE_RUN_USER www-data \
    APACHE_RUN_GROUP www-data \
    APACHE_LOCK_DIR /var/lock/apache2 \
    APACHE_LOG_DIR /var/log/apache2 \
    APACHE_PID_FILE /var/run/apache2/apache2.pid \
    APACHE_SERVER_NAME localhost 

COPY show-all.cgi /var/www/debug-certificate
COPY client-ca-auth.pem client-cgi.pem client-spcss.pem /etc/apache2/certs/
COPY log.conf hazard-ssl.conf /etc/apache2/conf-enabled/
ADD notify.cgi entry.sh certrenew-loop.sh / 

ENTRYPOINT /entry.sh

# zde jsou ulozeny certifikaty 
VOLUME /lego

# zde budete publikovat balicky
VOLUME /var/www/html/data

# v teto promenne predejte domenove jmeno vaseho serveru
ENV DOMAIN_NAME=

# v teto promenne uvedte email pro ucet u letsencrypt
ENV ADMIN_EMAIL=

