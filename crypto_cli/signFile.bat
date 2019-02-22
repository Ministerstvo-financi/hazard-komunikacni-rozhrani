@echo off

call %~dp0config.bat

set INPUT_FILE=%1
set OUTPUT_FILE=%2
set PKCS12_FILE=%3
set PKCS12_PWD=%4

java  -jar %~dp0crypto_cli-1.0-jar-with-dependencies.jar ^
      -f signFile ^
      -i %INPUT_FILE% ^
      -o %OUTPUT_FILE% ^
      -spks %PKCS12_FILE% ^
      -pass %PKCS12_PWD%