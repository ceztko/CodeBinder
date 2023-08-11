#!/usr/bin/env pwsh

$conf="Release"
dotnet build CodeBinder.sln --configuration $conf /p:Platform="Any CPU"

# This is needed as per https://github.com/dotnet/roslyn/issues/52293
dotnet restore "$((Join-Path Test CodeBinder.Test.sln))"

$codebinder = Join-Path bin $conf CodeBinder.exe

# Java JDK
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=Java --nsmapping=SampleLibrary:SampleLibrary `
    "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryJDK))"

# Java Android
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=Java --android --nsmapping=SampleLibrary:SampleLibrary `
    "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryAndroid))"

# Java JNI
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=JNI --nsmapping=SampleLibrary:SampleLibrary `
    "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryJNI))"

# ObjectiveC
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=ObjectiveC "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryObjC))"

# TypeScript (commonjs compatible)
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=TypeScript --commonjs --nsmapping=SampleLibrary:SampleLibrary `
    "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryTS))"

# TypeScript (ESModule compatible)
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=TypeScript --nsmapping=SampleLibrary:SampleLibrary `
    "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryMTS))"

# NodeJS NAPI
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=NAPI --nsmapping=SampleLibrary:SampleLibrary `
    "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryNAPI))"

# Project template conversions: creates the entry points for native methods

# CLang
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=CLang "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryCLang sgen))"

# EXPERIMENTAL: NativeAOT (partial method declarations)
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=NAOT "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryNAOT sgen))"
    
# EXPERIMENTAL: NativeAOT (partial method definitions)
& $codebinder `
    "--solution=$((Join-Path Test CodeBinder.Test.sln))" --project=SampleLibrary `
    --language=NAOT --create-template "--targetpath=$((Join-Path .. CodeBinder-TestCodeGen SampleLibraryNAOT))"
