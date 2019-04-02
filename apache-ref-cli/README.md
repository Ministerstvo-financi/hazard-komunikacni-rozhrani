# Instalace referenčního klienta
Referením klientem je sada nástrojů zveřejněných v tomto repozotáři, které slouží k výrobě datového balíčku 
a konfigurace webového serveru tak, aby z na něm mohly být balířky publikovány.  

Pozor - postupy uváděné v tomto návodu nemusí být dostatečné pro bezpečné zprovoznění instalace, která bude 
pracovat s citlivými daty. 

## Linux 
### Instalace docker a docker-compose
Na Linuxu použijeme pro instalaci serverové části referenčního klienta Docker. Nejprve na zvolený server [nainstalute docker](https://docs.docker.com/install/linux/docker-ce/ubuntu/#install-docker-ce): 

```
$ wget https://get.docker.com -O setup-docker.sh
$ sudo sh setup-docker.sh
$ sudo usermod -aG docker $USER
$ newgrp docker 
```

Nainstalujte docker-compose:
```
sudo wget "https://github.com/docker/compose/releases/download/1.23.2/docker-compose-$(uname -s)-$(uname -m)" -O /usr/local/bin/docker-compose
sudo chmod a+x /usr/local/bin/docker-compose
```

## Vlastnosti 
Předkonfigurované řešení je nastaveno tak, že musí být spuštěno na počítači, který je dostupný z internetu 
na konkrétním doménovém jméně. Toto jméno je nutno nastavit do proměnné prostředí `DOMAIN_NAME` před spuštěním kontejneru.

Kontejner si sám po spuštění zajistí vydání serverového certifikátu u certifikační autority [Let's Encrypt](https://letsencrypt.org/).
Pro interakci s touto autoritou je uvnitř kontejneru konfigurován klient [lego](https://github.com/go-acme/lego).

Pro interakci s LetsEncrypt je potřebná ještě email adresa, kterou předejte do kontejneru v proměnné prostředí `ADMIN_EMAIL`. 
Adresa zpravidla není k ničemu využívána.

Pokud je tedy například server dostupný na adrese https://dohled.hazarder.cz/, pak nastavení provedeme následujícícm způsobem:
```
$ export DOMAIN_NAME=dohled.hazarder.cz
$ export ADMIN_EMAIL=admin@hazarder.cz
$ docker-compose up -d
```

Po spuštění tohoto příkazu bude na počítači spuštěn webový server Apache, který naslouchá na standardním portu 443 takto:
* /data - je chráněná url, ke které má přístup pouze držitel nakonfigurovaných certifikátů; ve výchozím nastavení je povolen přístup 
  pro certifikáty, které MF používá k stahování dat v prostředí playground 
* /notify - je chráněná url určená pro příjem notifikací o výsledku zpracování balíčku; v základním nastavení pouze odpovídá Accepted! 

## Konfigurace
Nastavení proměnných prostředí je popsáno v předchozí kapitole. Pro úplné nastavení je nezbytné nastavit ještě následující: 
- certifikáty oprávněných klientů - certifikát ve formátu PEM je potřeba vložit do složky docker a podle vzoru zajistit jehopromítnutí 
  do výsledného spustitelného obrazu. 
- certifikát CA pro autentizaci klientů - nutno uložit do souboru client-ca-auth.pem. Pokud je autorit více, pak patří so souboru ve formátu 
  PEM jedna za druhou


## Publikování dat
Po správné konfiguraci a spuštění je možné publikovat data tak, že se odpovídající soubor nakopíruje do složky `data`.