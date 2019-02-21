set JAVA_HOME=C:\Program Files\Java\jdk-11.0.1
set PATH=%JAVA_HOME%\bin;%PATH%
java -Dlog4j.configuration=file:C:/testFiles/log4j.properties -jar target/crypto_cli-1.0.jar -f encryptFile -i C:/testFiles/testFiles/myDoc.txt -o C:/testFiles/encrypted-rsa.p7e -cert C:/testFiles/testFiles/rsa.pem -cert C:/testFiles/testFiles/rsa2.pem
pause