@echo off
setlocal enabledelayedexpansion

set "VSWHERECMD=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
for /f "usebackq tokens=*" %%i in (`"%VSWHERECMD%" -version 16.0 -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do (
	set "MSBUILDCMD=%%i"
)

"%MSBUILDCMD%" -version || exit /b 1
"%MSBUILDCMD%" CodeBinder.sln /p:Configuration=Release /p:Platform="Any CPU" /t:build /restore || exit /b 1
pause