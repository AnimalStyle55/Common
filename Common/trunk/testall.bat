@echo off

setlocal
packages\NUnit.Runners.2.6.4\tools\nunit-console.exe LD.Common.sln /framework:net-4.6.1 /noresult
if errorlevel 1 (
   echo TESTS FAILED!!
   exit /b %errorlevel%
)
endlocal
