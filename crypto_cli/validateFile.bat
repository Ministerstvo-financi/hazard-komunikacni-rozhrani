@echo off

call %~dp0config.bat

set INPUT_FILE=%1
set OUTPUT_FILE=%2
set EXPECTED_CERT1=%3
set EXPECTED_CERT2=%4

java -Djavax.net.ssl.trustStore=%~dp0cacerts.forAisg ^
     -Djavax.net.ssl.trustStorePassword=changeit ^
     -jar %~dp0crypto_cli-1.0-jar-with-dependencies.jar ^
     -f validateFile ^
     -i %INPUT_FILE% ^
     -o %OUTPUT_FILE% ^
     -cert %EXPECTED_CERT1% ^
     -cert %EXPECTED_CERT2%

