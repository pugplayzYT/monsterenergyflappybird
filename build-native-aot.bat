@echo off
setlocal
cd /d "%~dp0"
echo ========================================
echo  Monster Energy Flappy Bird - NativeAOT
echo ========================================
where dotnet >nul 2>nul
if errorlevel 1 (
  echo ERROR: .NET 10 SDK was not found.
  pause
  exit /b 1
)
dotnet publish MonsterEnergyFlappyBird.csproj -c Release -r win-x64 --self-contained true -o "%~dp0dist"
if errorlevel 1 (
  echo BUILD FAILED. NativeAOT also needs Visual Studio C++ Build Tools.
  pause
  exit /b 1
)
echo DONE: %~dp0dist\MonsterEnergyFlappyBird.exe
start "" "%~dp0dist"
pause
