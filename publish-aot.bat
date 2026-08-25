@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion
goto :ScriptMain

REM ============================================================================
REM  TCYM.UI.Example Native AOT 交叉编译脚本
REM
REM  使用 AotAnywhere（内置 Zig 工具链）交叉编译：
REM    - Windows x64 / arm64
REM    - Linux   x64 / arm64
REM    - macOS   x64 / arm64
REM
REM  用法：
REM    publish-aot.bat              编译全部平台
REM    publish-aot.bat win-x64      仅编译 win-x64
REM    publish-aot.bat win-arm64    仅编译 win-arm64
REM    publish-aot.bat linux-x64    仅编译 linux-x64
REM    publish-aot.bat linux-arm64  仅编译 linux-arm64
REM    publish-aot.bat osx-x64      仅编译 osx-x64
REM    publish-aot.bat osx-arm64    仅编译 osx-arm64
REM
REM  说明：
REM    - TCYM.UI 会按 RuntimeIdentifier 嵌入 SDL3 / SkiaSharp native 资源。
REM ============================================================================

:ScriptMain
set "PROJECT=%~dp0TCYM.UI.Example.csproj"
set "CONFIG=Release"
set "FRAMEWORK=net8.0"

if "%~1"=="" (
    set "TARGETS=win-x64 win-arm64 linux-x64 linux-arm64 osx-x64 osx-arm64"
) else (
    set "TARGETS=%~1"
)

echo ==========================================
echo   TCYM.UI.Example Native AOT publish
echo   Toolchain: AotAnywhere
echo   Targets: %TARGETS%
echo ==========================================
echo.

for %%T in (%TARGETS%) do (
    echo [build] %%T ...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Remove-Item -LiteralPath '%~dp0bin\%CONFIG%\%FRAMEWORK%\%%T' -Recurse -Force -ErrorAction SilentlyContinue"

    set "PUBLISH_ARGS="
    if /I "%%T"=="win-arm64" (
        call :SetupWinArm64Toolchain
        if !errorlevel! neq 0 (
            goto :ScriptFailed
        )
        set "PUBLISH_ARGS=-p:IlcUseEnvironmentalTools=true"
    )
    REM .NET 8 probes Xcode with clang on Windows; AotAnywhere links macOS through Zig, so skip the ld_classic probe.
    if /I "%%T"=="osx-x64" (
        set "PUBLISH_ARGS=!PUBLISH_ARGS! -p:UseLdClassicXCodeLinker=false"
    )
    if /I "%%T"=="osx-arm64" (
        set "PUBLISH_ARGS=!PUBLISH_ARGS! -p:UseLdClassicXCodeLinker=false"
    )

    dotnet publish "%PROJECT%" -r %%T -f %FRAMEWORK% -c %CONFIG% ^
        -p:StripSymbols=true ^
        -p:IlcOptimizationPreference=Size ^
        -p:TCYMEmbedSdl3Dynamic=true ^
        -p:TCYMCopySdl3Dynamic=false ^
        -p:TCYMEmbedSkiaSharpNative=true ^
        -p:TCYMCopySkiaSharpNative=false ^
        -p:TCYMEmbedCurrentRuntimeOnly=true ^
        -p:TCYMEmbeddedRuntimeIdentifier=%%T ^
        !PUBLISH_ARGS!

    set "PUBLISH_EXIT=!errorlevel!"
    if !PUBLISH_EXIT! neq 0 (
        echo [failed] %%T publish failed.
        goto :ScriptFailed
    )

    call :CleanPublishOutput %%T
    echo [ok] %%T publish completed.
    echo   Output: %~dp0bin\%CONFIG%\%FRAMEWORK%\%%T\publish\
    echo.
)

echo ==========================================
echo   All publishes completed.
echo ==========================================
echo.
for %%T in (%TARGETS%) do (
    echo %%T: %~dp0bin\%CONFIG%\%FRAMEWORK%\%%T\publish\
)

endlocal & exit /b 0

:ScriptFailed
endlocal & exit /b 1

:SetupWinArm64Toolchain
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not exist "%VSWHERE%" (
    echo [failed] vswhere.exe not found: "%VSWHERE%"
    exit /b 1
)

set "VS_INSTALL="
for /f "usebackq tokens=*" %%I in (`"%VSWHERE%" -latest -products * -property installationPath`) do set "VS_INSTALL=%%I"
if not defined VS_INSTALL (
    echo [failed] Visual Studio not found.
    exit /b 1
)

set "VCVARS_VERSION="
for /f "delims=" %%V in ('dir /b /ad /o-n "%VS_INSTALL%\VC\Tools\MSVC" 2^>nul') do (
    if not defined VCVARS_VERSION if exist "%VS_INSTALL%\VC\Tools\MSVC\%%V\bin\Hostx64\arm64\link.exe" set "VCVARS_VERSION=%%V"
)
if not defined VCVARS_VERSION (
    echo [failed] ARM64 link.exe not found under "%VS_INSTALL%\VC\Tools\MSVC".
    exit /b 1
)

call "%VS_INSTALL%\Common7\Tools\VsDevCmd.bat" -arch=arm64 -host_arch=x64 -vcvars_ver=%VCVARS_VERSION%
if %errorlevel% neq 0 (
    exit /b 1
)
echo [toolchain] win-arm64 MSVC %VCVARS_VERSION%
exit /b 0

:CleanPublishOutput
set "RID=%~1"
set "RID_PUBLISH_DIR=%~dp0bin\%CONFIG%\%FRAMEWORK%\%RID%\publish"
call :CleanOnePublishDir "%RID_PUBLISH_DIR%" %RID%
exit /b 0

:CleanOnePublishDir
set "PUBLISH_DIR=%~1"
set "RID=%~2"
if not exist "%PUBLISH_DIR%\" exit /b 0


for /r "%PUBLISH_DIR%" %%F in (*.pdb *.dbg *.xml *.lib) do (
    del /f /q "%%F" >nul 2>nul
)

echo [clean] %PUBLISH_DIR%
exit /b 0
