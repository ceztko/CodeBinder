call "%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars64.bat"
nuget restore CodeBinder.sln || exit /b 1
MSBuild CodeBinder.sln /p:Configuration=Release /p:Platform="Any CPU" /t:build /restore || exit /b 1
pause