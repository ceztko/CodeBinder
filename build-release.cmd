call "%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars64.bat"
MSBuild CodeBinder.sln /p:Configuration=Release /p:Platform="Any CPU" /t:build /restore || exit /b 1
pause