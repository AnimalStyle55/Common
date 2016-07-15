@echo off

setlocal
.nuget\NuGet.exe restore
"C:\Program Files (x86)\MSBuild\14.0\bin\MSBuild" LD.Common.sln /t:Rebuild /nologo /verbosity:minimal /logger:LD.Build.Logger.dll /p:Configuration=Debug
if errorlevel 1 (
   echo BUILD FAILED!!
   exit /b %errorlevel%
)
endlocal

