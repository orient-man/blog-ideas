@echo off
cls

.paket\paket.exe restore
if errorlevel 1 (
    exit /b %errorlevel%
)

SET SolutionDir=%~dp0
packages\FAKE\tools\FAKE.exe build.fsx %*