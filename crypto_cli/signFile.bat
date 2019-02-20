set JAVA_HOME=C:\Program Files\Java\jdk-11.0.1
set PATH=%JAVA_HOME%\bin;%PATH%
java -Dlog4j.configuration=file:C:/testFiles/log4j.properties -jar target/crypto_cli-1.0.jar -f signFile -i C:/testFiles/test.txt -o C:/testFiles/target/testdoc.p7m -spks C:/testFiles/user_a_rsa.p12 -pass password
pause