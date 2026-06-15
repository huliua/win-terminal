@echo off
echo Building WinTerminal...
echo.

REM Find MSBuild
set MSBUILD=
if exist "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" (
    set MSBUILD=C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe
) else if exist "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe" (
    set MSBUILD=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe
) else if exist "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" (
    set MSBUILD=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
)

if "%MSBUILD%"=="" (
    echo ERROR: MSBuild not found. Please install Visual Studio or .NET Framework SDK.
    pause
    exit /b 1
)

echo Using: %MSBUILD%
echo.

"%MSBUILD%" WinTerminal.sln /p:Configuration=Release /verbosity:minimal

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Build successful!
    echo Output: WinTerminal\bin\Release\WinTerminal.exe
) else (
    echo.
    echo Build failed with error code %ERRORLEVEL%
)

pause
