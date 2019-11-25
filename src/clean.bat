@echo off
rem Purpose of this is to clean out the bin, obj folders - in fact complete delete them!
rem Clean Build in Visual Studio does not do this, and sometimes it is necessary to do that properly!
@setlocal enableextensions enabledelayedexpansion

for /d /r . %%d in (bin,obj,.vs) do (
  set path=%%d
  set modified=!path:packages=X!
  if exist !path! (
    if !path!==!modified! (
      rd /s/q !path!
    )
  )
)