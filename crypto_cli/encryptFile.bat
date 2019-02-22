@echo off

call %~dp0config.bat

set INPUT_FILE=%1
set OUTPUT_FILE=%2
set RECIP_CERT1=%3
set RECIP_CERT2=%4

%JAVA_HOME%\bin\java -jar %~dp0crypto_cli-1.0-jar-with-dependencies.jar ^
     -f encryptFile ^
     -i %INPUT_FILE% ^
     -o %OUTPUT_FILE% ^
     -cert %RECIP_CERT1% ^
     -cert %RECIP_CERT2%
