@echo off

call %~dp0config.bat

set INPUT_FILE=%1
set OUTPUT_FILE=%2
set PRIVATE_KEY=%3
set RECIP_CERT=%4

java -jar %~dp0crypto_cli-1.0-jar-with-dependencies.jar ^
     -f decryptFile ^
     -i %INPUT_FILE% ^
     -o %OUTPUT_FILE% ^
     -k %PRIVATE_KEY% ^
     -cert %RECIP_CERT%
