@echo off
if /I %0 equ "%~dpnx0" cmd /K "%~dpnx0"

pwsh -File do-test-codegen.ps1