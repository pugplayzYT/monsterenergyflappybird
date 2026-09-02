@echo off
setlocal
cd /d "%~dp0"
echo Building normal self-contained Windows x64 version (NOT NativeAOT)...
dotnet publish MonsterEnergyFlappyBird.csproj -c Release -r win-x64 --self-contained true -p:PublishAot=false -o "%~dp0dist-normal"
if errorlevel 1 pause & exit /b 1
start "" "%~dp0dist-normal"
pause
