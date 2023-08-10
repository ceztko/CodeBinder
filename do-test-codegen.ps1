#!/usr/bin/env pwsh

$conf="Release"
dotnet build CodeBinder.sln --configuration $conf /p:Platform="Any CPU"

$codebinder = Join-Path bin $conf CodeBinder.exe

# Java JDK
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=Java --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryJDK))"

# Java Android
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=Java --android --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryAndroid))"

# Java JNI
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=JNI --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryJNI))"

# ObjectiveC
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=ObjectiveC "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryObjC))"

# TypeScript (commonjs compatible)
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=TypeScript --commonjs --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryTS))"

# TypeScript (ESModule compatible)
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=TypeScript --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryMTS))"

& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=NAPI --nsmapping=SampleLibrary:SampleLibrary `
    "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryMTS))"

# Project template conversions: creates the entry points for native methods

# CLang
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=CLang "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryCLang sgen))"

# EXPERIMENTAL: NativeAOT
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=NAOT "--rootpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryNAOT sgen))"
