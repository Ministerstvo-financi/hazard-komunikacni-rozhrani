# Referenční implementace Komunikačního rozhraní pro dohled nad Hazardem
V tomto repoziráři jsou shromážděny nástroje, které implementují funkce potřebné pro napojení 
provozovatelů hazardních her na Komunikační rozhraní za účelem poskytování automatizovaných výstupů 
podle [vyhlášky č.10/2019](https://www.zakonyprolidi.cz/cs/2019-10) o způsobu oznamování a zasílání informací 
a přenosu dat provozovatelem hazardních her, rozsahu přenášených dat a jiných technických parametrech přenosu dat.

Nástroje zveřejněné v tomto repozitáři umožňují naplnit požadavky vyhlášky následovně:
* `csv-validator` - ověření formální správnosti datového balíčku před zabalením, zašifrováním a zapečetěním
* `crypto_utils` a `crypto_cli` - Java knihovna a nástroj pro spuštění jednotlivých funkcí z příkazové řádky
    * vytváření uznávané elektronické pečeti datového balíčku
    * ověřování uznávané a kvalifikované elektronické pečeti potvrzovacího balíčku
    * šifrování datového balíčku před 
    * dešifrování potvrzovacího balíčku
* `apache-httpd-conf` - ukázková konfigurace HTTPS serveru provozovatele, která splňuje požadavky kladené komunikačním rozhraním

Nástroje jsou k dispozici již zkompilované pro spuštění na platformách Windows a Linux. Dále je k dispozici zdrojový kód pro použití v jiných aplikacích.
* `csv-validátor`
    * je implementován na platformě [.NET Core 2.2.](https://dotnet.microsoft.com/download/dotnet-core/2.2)
    * kompilované verze je možné spustit na 64 bit platformách Windows, Linux a MacOS X
* `crypto-utils` a `crypto-cli` 
    * je implementován v Javě 11 varianta OpenJDK
    * kompilované verze lze spustit všude, kde je k dispozici běhové prostředí [OpenJDK 11](https://openjdk.java.net/install/index.html)
* `apache-httpd-conf` - standardní HTTP server Apache http v 2.4. 
    * v prostředí Linuxu je typicky standardní součástí distribucí
    * v prostředí Windows je možné získat z některého z komunitních serverů [doporučovaných na stránkách Apache](https://httpd.apache.org/docs/current/platform/windows.html#down)

## Instalace 



## Postup přípravy balíčku
Předpoklady pro přípravu balíčku:
* Data datového balíčku ve formě CSV souborů ve složce jako např na následujícím výpisu
```
28934929-V-2019061208-B-01
├── evidence_her.csv
├── hra_toky.csv
├── hra_toky_oprava.csv
├── jedna_hra.csv
├── konto.csv
├── konto_zmeny.csv
├── mena_kurs.csv
├── ostatni_plneni.csv
├── ostatni_plneni_oprava.csv
├── prihlaseni.csv
├── provozovatel.csv
├── sebeomezeni.csv
├── sebeomezeni_hra_druh.csv
├── ucet.csv
└── vazba_hra_sazka.csv
```
* Funkční podporu Java 11
* Certifikát pro vytváření uznávané elektronické pečetě
    * ve formě PKCS12 souboru (přípona `pfx` nebo `p12`)
    * certifikát vydá některá z akreditovaných certifikačních autorit (eIdentity, I.CA, Postsignum) nebo libovolná zahraniční, 
      která splňuje požadavky nařízení eIDAS
    * Heslo k souboru s certifikátem pro uznávanou pečeť
* Šifrovací certifikát komunikačního rozhraní získaný z parametrů komunikace, které publikuje MF a CS
* Nástroj `zip` pro vytváření ZIP archivů (je například součástí Java 11 - `jar`)



## Sestavení
Sestavení je podporováno pouze na platfomě Linux. Výsledkem jsou binární výstupy pro všechny podporované platformy.
Pro sestavení musí stroj splňovat následující předpoklady:
* platforma OS Linux - testováno na Fedora Workstation 29, Ubuntu 18.04
* Docker Community Edition v18.06 
    * instalace dle postupu na [stránkách společnosti Docker](https://docs.docker.com/install/linux/docker-ce/)
* git - instalace standardní cestou balíčků distribuce
* konektivita do internetu, odkud se stahuje
    * docker obrazy, které slouží ke kompilaci komponent
    * baličky NuGet pro sestavení .NET Core komponent
    * balíčky Maven pro sestavení Java komponent

Jakmile jsou všechny předpoklady naplněny, je postup sestavení následující:
```
git clone https://github.com/Ministerstvo-financi/hazard-komunikacni-rozhrani.git
cd hazard-komunikacni-rozhrani
./build-docker.sh
```

Po dokončení sestavení jsou výsledné balíky ve složce `build`:
* crypto_utils.zip - JAR soubory obsahují knihovny a spustitelné modul, soubory typu BAT obsahují příklady, jak spouštět jednotlivé operace
  ```
	.
	├── crypto_cli-1.0.jar
	├── crypto_cli-1.0-jar-with-dependencies.jar
	├── crypto_utils-1.0.jar
	├── crypto_utils-1.0-jar-with-dependencies.jar
	├── decryptFile.bat
	├── encryptFile.bat
	├── signFile.bat
	├── validateCertificate.bat
	└── validateFile.bat

  ```
* CSV validátor - každá balík obsahuje všechny nezbytné soubory pro spuštění na dané platformě Validátor se spouští pomocí příkazu 
  `PackageValidation` (PackageValidation.exe pro windows apod.). Distribuční balíky pro jednotlivé platformy
	* `csv-validator-linux.zip` 
	* `csv-validator-macosx.zip`
	* `csv-validator-win.zip`

Postup instalace a použití takto sestavených komponent je shodný, jako postup pro komponenty distribuované v binárním tvaru,
které vznikají stejným způsbem.

# Licence
The referenční implementace tam, kde je vytvářen nový kód je licencována pod Apache License v2.0.
Referenční implementace obsahuje nebo využívá další komponenty, které jsou licencovány odpovídajícími 
licencemi:
* OpenSSL - [vlastní licence](https://www.openssl.org/source/license.html)
* Apache HTTPD - [Apache License v2.0](https://www.apache.org/licenses/LICENSE-2.0)
* DSS - [LGPL](https://github.com/esig/dss/blob/master/LICENSE) 
* .NET Core - [MIT](https://github.com/dotnet/core/blob/master/LICENSE.TXT)
* OpenJDK - [GPL](https://openjdk.java.net/legal/OCTLA-JDK9+.pdf)








