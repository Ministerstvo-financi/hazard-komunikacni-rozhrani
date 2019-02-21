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












# Licence
The referenční implementace tam, kde je vytvářen nový kód je licencována pod Apache License v2.0.
Referenční implementace obsahuje nebo využívá další komponenty, které jsou licencovány odpovídajícími 
licencemi:
* OpenSSL - [vlastní licence](https://www.openssl.org/source/license.html)
* Apache HTTPD - [Apache License v2.0](https://www.apache.org/licenses/LICENSE-2.0)
* DSS - [LGPL](https://github.com/esig/dss/blob/master/LICENSE) 
* .NET Core - [MIT](https://github.com/dotnet/core/blob/master/LICENSE.TXT)
* OpenJDK - [GPL](https://openjdk.java.net/legal/OCTLA-JDK9+.pdf)








