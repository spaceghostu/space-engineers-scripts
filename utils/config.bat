@echo off

set SE_DIR=%userprofile%\AppData\Roaming\SpaceEngineers
set SE_SCRIPTS_DIR=%SE_DIR%\IngameScripts\local
set CS=Script.cs
set PNG=thumb.png

set git_dir=C:\Program Files\Git

@REM for /f "usebackq delims=" %%i in (`where git 2^>NUL`) do (
@REM   set git_dir=%%~dpi..
@REM )

if "%git_dir%" == "" (
  echo Git not found.
  exit /b 1
)

set sed=%git_dir%\usr\bin\sed.exe

if not exist "%sed%" (
  echo Not found: "%sed%".
  exit /b 2
)

if not exist "%SE_DIR%" (
  echo Space Engineers AppData not found: "%SE_DIR%".
  exit /b 3
)
