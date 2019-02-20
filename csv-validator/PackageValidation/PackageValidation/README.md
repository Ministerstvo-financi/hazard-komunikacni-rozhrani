# Validátor datového balíčku 

## Instalace a spuštění
Po stažení odpovídající verze distribučního balíčku podle platformy (linux-x64, win-x64) balíček rozbalte do složky. 

Je nutno nakonfigurovat složku, kam se bude ukládat globální log (viz níže v kapitole Logování) a tuto složku vytvořit.

Validátor se spouští z příkazové řádky:

Linux
```
$>./PackageValidation složka-s-obsahem-balíčku 
```

Windows
```
c:>PackageValidation.exe složka-s-obsahem-balíčku
```

Příklad spuštění:
```
c:\>PackageValidation.exe c:\packages\123456789-V-2019070108-L-01
```

## Logování
Aplikace produkuje 4 výstupní soubory:
* globální aplikační log - je ukládán do souboru určeného konfiguračním parametrem 
  načítaným ze souboru `structureSourceSettings.json` z položky `GlobalLogDir` a obsahuje 
  všechny chyby a informace, které jsou globálního charakteru. Složka musí existovat. Pokud konfigurujete složku na windows, 
  musí být zpětná lomítka zdvojena (jedná se o JSON soubor)
* průběh validace balíčku - soubor `validation-processing.log` vznikne přímo ve složce balíčku, kam se loguje vše po zajištění dostupnosti složky balíčku
* chyby validace - `validation-errors.txt` vznikne přímo ve složce balíčku - CSV soubor se strukturovanými výpisy chybových hlášení selhaných validací
* výsledek validace - `validation-result.txt` vznikne přímo ve složce balíčku- CSV soubor, který obsahuje výsledek validací jednotlivých CSV souborů ve formě CSV



