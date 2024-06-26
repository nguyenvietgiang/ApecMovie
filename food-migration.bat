@ECHO OFF

set COMMAND=%1

for /D %%I in ("ApecFoodService.API") do set ApiDirName=%%~nxI
echo %ApiDirName%

for /D %%I in ("ApecFoodService.Infrastructure") do set DataDirName=%%~nxI
echo %DataDirName%

if "%COMMAND%"=="" (
    SET /P COMMAND="Choose dotnet ef command (add, remove, update, list, revert): "
)

2>NUL CALL :CASE_%COMMAND%
IF ERRORLEVEL 1 CALL :DEFAULT_CASE

echo Done.
exit /B

:CASE_add
    cd ./%ApiDirName%
    SET /P MIGRATION="Name of migration: "
    dotnet ef migrations add %MIGRATION% --project "../%DataDirName%/%DataDirName%.csproj" --startup-project "%ApiDirName%.csproj"
    cd ..
    GOTO END_CASE

:CASE_remove
    cd ./%ApiDirName%
    dotnet ef migrations remove --project "../%DataDirName%/%DataDirName%.csproj" --startup-project "%ApiDirName%.csproj"
    cd ..
    GOTO END_CASE

:CASE_update
    cd ./%ApiDirName%
    dotnet ef database update --project "../%DataDirName%/%DataDirName%.csproj" --startup-project "%ApiDirName%.csproj"
    cd ..
    GOTO END_CASE

:CASE_revert
    cd ./%ApiDirName%
    SET /P MIGRATION_REVERT="Name of previous migration: "
    dotnet ef database update "%MIGRATION_REVERT%" --project "../%DataDirName%/%DataDirName%.csproj" --startup-project "%ApiDirName%.csproj"
    cd ..
    GOTO END_CASE

:CASE_list
    cd ./%ApiDirName%
    dotnet ef migrations list --project "../%DataDirName%/%DataDirName%.csproj" --startup-project "%ApiDirName%.csproj"
    cd ..
    GOTO END_CASE

:DEFAULT_CASE
    ECHO Unknown command "%COMMAND%"
    GOTO END_CASE

:END_CASE
    echo "end run migration"
    pause
    VER > NUL
    GOTO :EOF