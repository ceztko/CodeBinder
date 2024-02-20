#!/usr/bin/env pwsh

$PSNativeCommandUseErrorActionPreference = $true
$ErrorActionPreference = 'stop'

dotnet pack CodeBinder.Redist.slnf