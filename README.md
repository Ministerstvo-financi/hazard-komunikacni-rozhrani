# Referenční klient komunikačního rozhraní dle vyhlášky č. 10/2019
V tomto repozitáři jsou shromážděny vzorové nástroje, které implementují funkce potřebné pro napojení provozovatelů hazardních her za účelem poskytování automatizovaných výstupů podle [vyhlášky č.10/2019](https://www.zakonyprolidi.cz/cs/2019-10) o způsobu oznamování a zasílání informací a přenosu dat provozovatelem hazardních her, rozsahu přenášených dat a jiných technických parametrech přenosu dat.

Nástroje, dokumentace a další materiály v tomto repozitáři jsou poskytovány jako pomocný nástroj pro provozovatele hazardních her pro usnadnění implementace rozhraní, bez záruky funkčnosti a nenahrazují normativní znění vyhlášky. Opravy případných chyb a odlišností od normativního znění vyhlášky budou opraveny a publikovány stejným způsobem.

# Přehled
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

# Instalace 
Distribuční soubor stahněte z [GitHub](https://github.com/Ministerstvo-financi/hazard-komunikacni-rozhrani/releases). 
Distribuční soubor ZIP rozbalte na svém počítači. Následně je možné využít postupu níže k vytvářoření datového balíčku. 

# Postup přípravy balíčku
## Předpoklady pro přípravu balíčku
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

Po rozbalení dritibučního balíku vznikne následující struktura - pro následující postupy předpokládáme, že tato struktura je umístena ve složce `C:\hazard`, na linuxu pak složka `/hazard`:
```
C:\hazard
├── crypto_utils
│   ├── config.bat
│   ├── crypto_cli-1.0.jar
│   ├── crypto_cli-1.0-jar-with-dependencies.jar
│   ├── crypto_utils-1.0.jar
│   ├── crypto_utils-1.0-jar-with-dependencies.jar
│   ├── decryptFile.bat
│   ├── encryptFile.bat
│   ├── signFile.bat
│   ├── validateCertificate.bat
│   └── validateFile.bat
├── csv-validator-win
│   ├── api-ms-win-core-console-l1-1-0.dll
│   ├── .... dalsi ...dll
│   ├── api-ms-win-crt-utility-l1-1-0.dll
│   ├── App.config
│   ├── README.md
│   ├── RELEASE-NOTES.md
│   ├── SchemaSource
│   │   ├── codebook.csv
│   │   ├── fields_structure.csv
│   │   ├── game_type_ref.csv
│   │   ├── model_file_mandatory.csv
│   │   ├── model_game_file.csv
│   │   ├── structured-schema-description.xlsx
│   │   ├── Validace-balicku.docx
│   │   └── Validace-balicku-EN.docx
│   ├── PackageValidation.exe
│   └── structureSourceSettings.json
└── data
    ├── 28934929-V-2019061208-B-01
    │   ├── evidence_her.csv
    │   ├── hra_toky.csv
    │   ├── hra_toky_oprava.csv
    │   ├── jedna_hra.csv
    │   ├── konto.csv
    │   ├── konto_zmeny.csv
    │   ├── mena_kurs.csv
    │   ├── ostatni_plneni.csv
    │   ├── ostatni_plneni_oprava.csv
    │   ├── prihlaseni.csv
    │   ├── provozovatel.csv
    │   ├── sebeomezeni.csv
    │   ├── sebeomezeni_hra_druh.csv
    │   ├── ucet.csv
    │   └── vazba_hra_sazka.csv
    ├── mf
    │   ├── ca.crt
    │   ├── mf-aisg-sifrovaci.crt
    │   ├── mf-aisg-sifrovaci.key
    │   ├── mf-system-aisg.crt
    │   ├── mf-system-aisg.key
    │   └── mf-system-aisg.p12
    └── test-hazard
        ├── ca.crt
        ├── test-hazard-simulace-pecet.p12
        ├── test-hazard-simulace-pecet.crt
        ├── test-hazard-simulace-pecet.key
        ├── test-hazard-sifrovani.crt
        ├── test-hazard-sifrovani.key
        └── test-hazard-sifrovani.p12
```


## Postup vytvoření datového balíčku

### Validace datového obsahu balíčku 
**Windows:**

Spustit příkazovou řádku `cmd.exe`. Validátor se spustí pomocí následujících příkazů:
```bat
C:
cd c:\hazard
csv-validator-win\PackageValidation.exe  data\28934929-V-2019061208-B-01
```
V průběhu validace vznikají následující soubory:
* `data\28934929-V-2019061208-B-01\validation-results.csv` - celkový výsledek validace jednotlivých CSV
* `data\28934929-V-2019061208-B-01\validation-processing.txt` - záznam průběhu zpracování validaci
* `data\28934929-V-2019061208-B-01\validation-errors.csv` - seznam chyb nalezených ve všech CSV souborech
* `global.log` - globální log, který obsahuje především informace mimo samotné zpracování balíčku. Umístění tohoto souboru 
  je určeno hodnotou `GlobalLog` v souboru  `csv-validator-win\structureSourceSettings.json`

Pokud je balíček bez chyb, jsou všechny řádky v souboru `validation-results.csv` označeny jako `VALID` 
a soubor `validation-errors.csv` je prázdný.

**Linux:**

Spustit příkazový řádek 
```
TBD
```

### Vytvořní ZIPu
**Windows:**

Spustit Powershell a zadat následující příkazy
```bat
C:
cd c:\hazard
cd data\28934929-V-2019061208-B-01
Compress-Archive -Path *.csv -DestinationPath ..\28934929-V-2019061208-B-01
```
Ve složce `c:\hazard\data` vznikne soubor `28934929-V-2019061208-B-01.zip`

**Linux:**

Spustit příkazový řádek 
```
TBD
```

### Šifrování
**Windows**

Soubor `crypto_utils\config.bat` změnit tak, aby byla správně nastavena proměnná JAVA_HOME (testováno jen s umístěním bez mezer v názvech složek).
Demonstrační data obsahují soubory:
* `mf-aisg-sifrovaci.crt` - šifrovací certifikát MF, který provozvatel získá z publikovaných údajů pro zabezpečení komunikace 
   z JSON položky `certifikatySifrovani`
* `test-hazard-sifrovani.crt` - šifrovací certifikát jehož privátní klíč je ve vlastnictví provozovatele - certifikát provozovatel registruje 
   jako součást svých údajů pro technické zabezpečení komunikace prostředictvím formuláře na stránkých Celní správy jako Šifrovací certifikát

Spustit příkazovou řádku `cmd.exe`. Šifrování dat na Windows probíhá pomocí následujících příkazů:
```bat
C:
cd C:\hazard
crypto_utils\encryptFile.bat data\28934929-V-2019061208-B-01.zip data\28934929-V-2019061208-B-01.zip.p7e data\mf\mf-aisg-sifrovaci.crt data\test-hazard\test-hazard-sifrovani.crt
```
Vznikne soubor `data\28934929-V-2019061208-B-01.zip.p7e`, který je možné dešifrovat klíči náležejícími k certifikátům `mf-aisg-sifrovaci.crt` a `test-hazard-sifrovani.crt` - tedy certifikátem MF a také certifikátem provozvatele. Certifikát provozovatele není nezbytné při šifrování použít. Je to však obvyklá praxe, kdy je odesílatel schopen data dešifrovat. 

**Linux:**

Spustit příkazový řádek 
```
TBD
```


### Vytvoření elektronické pečeti
**Windows**

Demonstrační data obsahují soubory:
* `test-hazard\test-hazard-simulace-pecet.p12` - jedna se o certifikát, kterým v následujicím postupu simulováno použití 
  kvalifikovaného certifikátu pro tvorbu uznávané pečeti. Tento certifikát musí být pro použití v prostředí playground 
  nahrazen certifikátem, který vydá akreditovaná certifikační autorita (nikoli testovací certifikát)  
Spustit příkazovou řádku `cmd.exe`. Pečetění dat na Windows probíhá pomocí následujících příkazů (POZOR - poslední cesta musí používat 
dopředná lomítka in na windows):
```bat
C:
cd C:\hazard
crypto_utils\signFile.bat data\28934929-V-2019061208-B-01.zip.p7e data\28934929-V-2019061208-B-01.zip.p7e.p7s data/test-hazard/test-hazard-simulace-pecet.p12 12345678
```
Vznikne soubor `data\28934929-V-2019061208-B-01.zip.p7e.p7s`. Tento soubor je již výsledným datovým balíčkem, který provozovatel vystaví ke stažení.

**Linux:**

Spustit příkazový řádek 
```
TBD
```

### Postup zpracování potvrzovacího balíčku
#### Validace pečeti potvrzovacího balíčku
**Windows**
Pro validaci pečeti potvrzovacího balíčku je nezbytné získat certifikát, který komunikační rozhraní používá 
k vytváření elektornických pečetí. MF publikuje parametry komunikačního rozhraní na stránkách věnovaných [reportingu 
v oblasti hazardních her](https://www.mfcr.cz/hazard). 

Pro příklad validace použijeme [publikované parametry pro prostředí playground](https://www.mfcr.cz/cs/soukromy-sektor/hazardni-hry/reportingova-vyhlaska/komunikacni-parametry/komunikacni-parametry-pro-playground). Parametry jsou publikovány jednak přímo na odkazované webové stránce. Autoritativním zdrojem 
komunikačních parametrů je strojově čitelný soubor JSON, který obsahuje všechny podstatné certifikáty a URL. Soubor s komunikačními parametry je vystavený 
[na stránkách ve formě ZIP souboru](https://www.mfcr.cz/assets/cs/media/2b80e14e53e7a24b71c8153e71cbfae5bcf4c42789d4ec67c67f76122446df00.zip), který obsahuje jednak přímo JSON soubor a zároveň JSON soubor zapečetěný. Jako ukázku validace pečeti a extrakce zapečetěných dat použijeme soubor stažený z uvedeného URL.

Pro validaci pečeti souboru je možné použít `/crypto_utils/validateFile.bat INPUT OUTPUT CERT1 CERT2` s následujícími parametry:
* INPUT - zapečetěný soubor, jehož pečeť validujeme
* OUTPUT - výsledek ověření pečeti - data, která jsou v souboru INPUT zapečetěna
* CERT1 - certifikát, kterým se očekává, že je pečeť vytvořena - první kandidát
* CERT2 - certifikát, kterým se očekává, že je pečeť vytvořena - druhý kandidát - může být shodný s prvním, pokud existuje jen jeden možný certifikát, 
  kterým má být zapečetěno

Konkrétně pro parametry Playgroundu jde o následující soubory:
* INPUT: 2b80e14e53e7a24b71c8153e71cbfae5bcf4c42789d4ec67c67f76122446df00.json.p7s - získaný ze [staženéo ZIP souboru](https://www.mfcr.cz/assets/cs/media/2b80e14e53e7a24b71c8153e71cbfae5bcf4c42789d4ec67c67f76122446df00.zip)
* OUTPUT: 2b80e14e53e7a24b71c8153e71cbfae5bcf4c42789d4ec67c67f76122446df00.json - soubor, kam bude zapsán výsledek
* CERT1: spcss-pg-pecet.der - soubor pro pečetění získaný např. [ze ZIP souboru](https://www.mfcr.cz/assets/cs/media/Prehled_Certifikaty-organu-vykonavajici-dozor-pro-Playground.zip)
* CERT2: spcss-pg-pecet.der - existuje pouze jeden správný certifikát, proto ho pro spuštění zdvojíme


Před spuštěním je potřeba ještě ověřit, zda je správně nastavena cesta k java.exe v souboru `config.bat`.

Výsledný příkaz pro validaci pečeti bude vypadat takto (^ na konci řádku umožňuje zpasat jede příkaz na více řádků):
```
> cd c:\hazard
> crypto_utils\validateFile.bat 2b80e14e53e7a24b71c8153e71cbfae5bcf4c42789d4ec67c67f76122446df00.json.p7s ^
  2b80e14e53e7a24b71c8153e71cbfae5bcf4c42789d4ec67c67f76122446df00.json ^
  spcss-pg-pecet.der ^
  spcss-pg-pecet.der 
```
Spustí se validace, která nejprve provádí načtení EU Trust listu. Poté proběhne validace podpisu/pečeti, extrakce podepsaných/pečetěných dat a výpis výsledku validace.

```
2019-03-09 23:57:16 INFO  DssCommands:57 - Starting validateFile
2019-03-09 23:57:16 ERROR DssValidator:245 - FAILED to read keystore file - using default keystore
[main] INFO eu.europa.esig.dss.tsl.service.TSLRepository - New version of EU TSL is stored in cache
WARNING: An illegal reflective access operation has occurred
WARNING: Illegal reflective access by com.sun.xml.bind.v2.runtime.reflect.opt.Injector (file:/T:/proj/SPCSS/2018/analyticky-modul/repo/hazard-komunikacni-rozhrani/build/hazard-2019-03-10.1552227930/crypto_utils/crypto_cli-1.0-jar-with-dependencies.jar) to method java.lang.ClassLoader.defineClass(java.lang.String,byte[],int,int)
WARNING: Please consider reporting this to the maintainers of com.sun.xml.bind.v2.runtime.reflect.opt.Injector
WARNING: Use --illegal-access=warn to enable warnings of further illegal reflective access operations
WARNING: All illegal access operations will be denied in a future release
[main] INFO eu.europa.esig.dss.tsl.service.TSLRepository - New version of the pivot LOTL 'https://ec.europa.eu/information_society/policy/esignature/trusted-list/tl-pivot-172-mp.xml' is stored in cache
[pool-1-thread-3] INFO eu.europa.esig.dss.validation.CommonCertificateVerifier - + New CommonCertificateVerifier created.
[pool-1-thread-3] INFO eu.europa.esig.dss.validation.SignedDocumentValidator - Validator 'eu.europa.esig.dss.asic.validation.ASiCContainerWithCAdESValidator' is registred
[pool-1-thread-3] INFO eu.europa.esig.dss.validation.SignedDocumentValidator - Validator 'eu.europa.esig.dss.pades.validation.PDFDocumentValidator' is registred
...
...
...
...
[main] INFO eu.europa.esig.dss.tsl.service.TSLRepository - Nb of loaded trusted lists : 30/32
[main] INFO eu.europa.esig.dss.tsl.service.TSLRepository - Nb of trusted certificates : 2403
[main] INFO eu.europa.esig.dss.tsl.service.TSLRepository - Nb of trusted public keys : 2311
[main] INFO eu.europa.esig.dss.validation.CommonCertificateVerifier - + New CommonCertificateVerifier created.
[main] INFO eu.europa.esig.dss.validation.SignedDocumentValidator - Document validation...
[main] INFO eu.europa.esig.dss.x509.ocsp.OCSPToken - OCSP status is good
2019-03-09 23:59:21 INFO  Main:74 - ValidateFile INFO_PKG_SIG_CERT_OK
```  
V případě úspěšné validace bude také vytvořen OUTPUT soubor.

Stejným způsobem je možné ověřit pečeti potvrzovacího balíčku tak, že INPUT soubor v přípazech bude jménem získaného potvrzovacího balíčku a OUTPUT bude název souboru šifrovaného balíčku po ověření pečeti. Příkaz bude vypadat např. takto:
```
> cd c:\hazard
> crypto_utils\validateFile.bat 28934929-V-2019061208-B-01.Ok.zip.p7e.p7s ^
  28934929-V-2019061208-B-01.Ok.zip.p7e ^
  spcss-pg-pecet.der ^
  spcss-pg-pecet.der 
```

#### Dešifrování potvrzovacího balíčku
```
> cd c:\hazard
> crypto_utils\decryptFile.bat 28934929-V-2019061208-B-01.Ok.zip.p7e^
  28934929-V-2019061208-B-01.Ok.zip ^
  encryption-private.key ^
  encryption-cert.pem 
```


**Poznámka:** Pro získání klíče ve správném formátu ze souboru PKCS12 pro uvedený příkaz je možné použít následující příkazy:

Export privátního klíče:
```
openssl pkcs12 -in test-hazard-sifrovani.p12 -nodes -nocerts -out encryption-private.key
```

Export certifikátu:
```
openssl pkcs12 -in test-hazard-sifrovani.p12 -nodes -nokeys -out encryption-cert.pem 
```

#### Rozzipování potvrzovacího balíčku

TBD



## Sestavení modulů
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
```shell
git clone https://github.com/Ministerstvo-financi/hazard-komunikacni-rozhrani.git
cd hazard-komunikacni-rozhrani
./build-docker.sh
```
Po dokončení sestavení jsou výsledné balíky ve složce `build` ve strukture, jak je uvedeno výše. 

Postup instalace a použití takto sestavených komponent je shodný, jako postup pro komponenty distribuované v binárním tvaru,
které vznikají stejným způsbem.

### OpenSSL Windows binární
Binární verze OpenSSL, která je součástí distribučního balíčku, je stažena z https://kb.firedaemon.com/support/solutions/articles/4000121705-openssl-binaries

# Licence
The referenční implementace tam, kde je vytvářen nový kód je licencována pod Apache License v2.0.
Referenční implementace obsahuje nebo využívá další komponenty, které jsou licencovány odpovídajícími 
licencemi:
* OpenSSL - [vlastní licence](https://www.openssl.org/source/license.html)
* Apache HTTPD - [Apache License v2.0](https://www.apache.org/licenses/LICENSE-2.0)
* DSS - [LGPL](https://github.com/esig/dss/blob/master/LICENSE) 
* .NET Core - [MIT](https://github.com/dotnet/core/blob/master/LICENSE.TXT)
* OpenJDK - [GPL](https://openjdk.java.net/legal/OCTLA-JDK9+.pdf)








