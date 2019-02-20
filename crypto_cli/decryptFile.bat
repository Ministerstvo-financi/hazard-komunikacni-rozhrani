set JAVA_HOME=C:\Program Files\Java\jdk-11.0.1
set PATH=%JAVA_HOME%\bin;%PATH%
java -Dlog4j.configuration=file:C:/testFiles/log4j.properties -jar target/crypto_cli-1.0.jar -f decryptFile -i C:/testFiles/testFiles/encrypted-rsa.p7e -o C:/testFiles/myDoc-rsa-dec.txt -k C:/testFiles/testFiles/rsa.key -cert C:/testFiles/testFiles/rsa.pem
pause