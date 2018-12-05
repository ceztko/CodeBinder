call "%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Community\VC\Auxiliary\Build\vcvars64.bat"
nuget restore CodeBinder.sln
MSBuild CodeBinder.sln /p:Configuration=Release /p:Platform="Any CPU" /t:build /restore
pause