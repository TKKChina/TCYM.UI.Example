@echo off
chcp 65001 >nul
setlocal
goto :ScriptMain

REM ============================================================================
REM TCYM.UI.Example Native AOT 跨平台发布脚本
REM
REM 工具链：
REM   使用 AotAnywhere，并由它自动还原/使用内置 Zig 工具链。
REM    - Windows x64 / arm64
REM    - Linux   x64 / arm64
REM    - macOS   x64 / arm64
REM
REM PowerShell 用法：
REM    .\publish-aot.bat              编译全部平台
REM    .\publish-aot.bat win-x64      仅编译 win-x64
REM    .\publish-aot.bat win-arm64    仅编译 win-arm64
REM    .\publish-aot.bat linux-x64    仅编译 linux-x64
REM    .\publish-aot.bat linux-arm64  仅编译 linux-arm64
REM    .\publish-aot.bat osx-x64      仅编译 osx-x64
REM    .\publish-aot.bat osx-arm64    仅编译 osx-arm64
REM
REM 参数说明：
REM   不传 RID：发布全部平台。
REM   指定 RID：只发布指定平台，例如 win-x64、linux-x64、osx-arm64。
REM
REM 注意：
REM   TCYM.UI 会按 RuntimeIdentifier 嵌入 SDL3 和 SkiaSharp native 资源。
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

for %%T in (%TARGETS%) do call :PublishOne %%T
if errorlevel 1 goto :ScriptFailed

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

:PublishOne
set "RID=%~1"
set "PUBLISH_ARGS="

echo [build] %RID% ...
powershell -NoProfile -ExecutionPolicy Bypass -Command "Remove-Item -LiteralPath '%~dp0bin\%CONFIG%\%FRAMEWORK%\%RID%' -Recurse -Force -ErrorAction SilentlyContinue"

if /I "%RID%"=="win-arm64" (
    call :SetupWinArm64Toolchain
    if errorlevel 1 exit /b 1
    set "PUBLISH_ARGS=-p:IlcUseEnvironmentalTools=true"
)

REM .NET 8 在 Windows 上发布 macOS RID 时会用 clang 探测 Xcode；AotAnywhere 实际用 Zig 链接，所以跳过这个探测。
if /I "%RID%"=="osx-x64" (
    set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:UseLdClassicXCodeLinker=false"
)
if /I "%RID%"=="osx-arm64" (
    set "PUBLISH_ARGS=%PUBLISH_ARGS% -p:UseLdClassicXCodeLinker=false"
)

dotnet publish "%PROJECT%" -r %RID% -f %FRAMEWORK% -c %CONFIG% ^
    -p:StripSymbols=true ^
    -p:IlcOptimizationPreference=Size ^
    -p:TCYMEmbedSdl3Dynamic=true ^
    -p:TCYMCopySdl3Dynamic=false ^
    -p:TCYMEmbedSkiaSharpNative=true ^
    -p:TCYMCopySkiaSharpNative=false ^
    -p:TCYMEmbedCurrentRuntimeOnly=true ^
    -p:TCYMEmbeddedRuntimeIdentifier=%RID% ^
    %PUBLISH_ARGS%

if errorlevel 1 (
    echo [failed] %RID% publish failed.
    exit /b 1
)

call :CleanPublishOutput %RID%
echo [ok] %RID% publish completed.
echo   Output: %~dp0bin\%CONFIG%\%FRAMEWORK%\%RID%\publish\
echo.
exit /b 0

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
if errorlevel 1 (
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