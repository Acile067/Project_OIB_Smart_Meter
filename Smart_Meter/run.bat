@echo off

REM Postavite lozinke
set MAIN_PASS=1234
set LB_PASS=1234
set WORKER1_PASS=1234
set WORKER2_PASS=1234
set OPERATOR1_PASS=1234

REM Pokrenite aplikacije
echo Pokretanje Service kao MainService...
PsExec -d -u MainService -p %MAIN_PASS% "%~dp0Service\bin\Debug\Service.exe"
timeout /t 1 >nul

echo Pokretanje LoadBalancer kao LoadBalancer...
PsExec -d -u LoadBalancer -p %LB_PASS% "%~dp0LoadBalancer\bin\Debug\LoadBalancer.exe"
timeout /t 1 >nul

echo Pokretanje Worker1 kao Worker1...
PsExec -d -u Worker1 -p %WORKER1_PASS% "%~dp0Worker\bin\Debug\Worker.exe"
timeout /t 1 >nul

echo Pokretanje Worker2 kao Worker2...
PsExec -d -u Worker2 -p %WORKER2_PASS% "%~dp0Worker\bin\Debug\Worker.exe"
timeout /t 1 >nul

echo Pokretanje Client kao Operator1...
PsExec -d -u Operator1 -p %OPERATOR1_PASS% "%~dp0Client\bin\Debug\Client.exe"

pause
