@echo off
@echo. del bin and obj

for /d /r . %%d in (bin,obj) do @if exist "%%d" rd /s/q "%%d"

@echo.

@echo. any key exit.
@echo.
pause > nul