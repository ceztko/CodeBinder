#!/usr/bin/env pwsh

$conf="Release"
dotnet build CodeBinder.sln --configuration $conf /p:Platform="Any CPU"

$codebinder = Join-Path bin $conf CodeBinder.exe
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=Java --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryJDK))"
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=Java --android --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryAndroid))"
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=JNI --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryJNI))"
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=ObjectiveC "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryObjC))"
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=TypeScript --commonjs --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryTS))"
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=TypeScript --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryMTS))"
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=CLang "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryCLang sgen))"
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=NativeAOT "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryNAOT sgen))"