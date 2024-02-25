#!/usr/bin/env pwsh

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'stop'

dotnet pack /p:DebugType=None /p:DebugSymbols=false CodeBinder.CLI.slnf