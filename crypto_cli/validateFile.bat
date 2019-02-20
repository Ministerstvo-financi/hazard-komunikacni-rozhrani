set JAVA_HOME=C:\Program Files\Java\jdk-11.0.1
set PATH=%JAVA_HOME%\bin;%PATH%
java -Dlog4j.configuration=file:C:/testFiles/log4j.properties -jar target/crypto_cli-1.0.jar -f validateFile -i C:/testFiles/signedFiles/dss-test-signed-cades-baseline-b.pkcs7 -o C:/testFiles/target/result.txt -cert C:/testFiles/keystore/ec.europa.eu.1.cer -cert C:/testFiles/keystore/ec.europa.eu.2.cer
pause