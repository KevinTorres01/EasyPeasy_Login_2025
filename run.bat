@echo off
REM EasyPeasy Login - Windows Startup Script

setlocal enabledelayedexpansion

set PROJECT_DIR=%~dp0
set WEB_PROJECT=%PROJECT_DIR%src\EasyPeasy_Login.Web

echo 🚀 EasyPeasy Login - Starting Application
echo ==========================================
echo.

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET SDK is not installed!
    echo Please install .NET 10.0 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo ✓ .NET SDK found
for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo   Version: %DOTNET_VERSION%
echo.

REM Clean previous builds
echo 🧹 Cleaning previous builds...
if exist "%PROJECT_DIR%bin" rmdir /s /q "%PROJECT_DIR%bin" >nul 2>&1
for /d /r "%PROJECT_DIR%src" %%D in (bin) do if exist "%%D" rmdir /s /q "%%D" >nul 2>&1
for /d /r "%PROJECT_DIR%src" %%D in (obj) do if exist "%%D" rmdir /s /q "%%D" >nul 2>&1
echo ✓ Clean complete
echo.

REM Build the solution
echo 🔨 Building solution...
cd /d "%PROJECT_DIR%"
dotnet build -c Debug

if errorlevel 1 (
    echo ❌ Build failed!
    pause
    exit /b 1
)

echo ✓ Build successful
echo.

REM Display application info
echo 📋 Application Ports ^& Access:
echo   ├─ HttpServer (Captive Portal):  http://192.168.100.1:8080
echo   │  ├─ Login Page:                http://192.168.100.1:8080/portal/login
echo   │  ├─ Success Page:              http://192.168.100.1:8080/portal/success
echo   │  └─ Default Admin User:        admin / admin05
echo   │
echo   └─ ASP.NET Admin Panel:         http://192.168.100.1:5000
echo      └─ Admin Dashboard:           http://192.168.100.1:5000/admin
echo      └─ Blazor UI on port 5000
echo.

REM Start the application
echo ▶️  Starting application...
echo Note: The application will run in the foreground.
echo Press Ctrl+C to stop.
echo.

cd /d "%WEB_PROJECT%"
dotnet run -c Debug

pause
