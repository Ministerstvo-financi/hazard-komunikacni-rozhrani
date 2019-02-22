@echo off

call %~dp0config.bat

set INPUT_FILE=%1

java -jar %~dp0crypto_cli-1.0-jar-with-dependencies.jar ^
     -f validateCertificate ^
     -i %INPUT_FILE%
