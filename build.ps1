# SAST Image Client 编译脚本
# 自动设置 UTF-8 编码并编译项目

param(
    [string]$Platform = "x64",
    [switch]$Clean,
    [switch]$Rebuild
)

# 设置控制台编码为 UTF-8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
chcp 65001 | Out-Null

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SAST Image Client 编译工具" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 显示编译配置
Write-Host "编译配置：" -ForegroundColor Yellow
Write-Host "  平台: $Platform" -ForegroundColor White
Write-Host "  配置: Debug" -ForegroundColor White
Write-Host ""

# 清理项目
if ($Clean -or $Rebuild) {
    Write-Host "正在清理项目..." -ForegroundColor Yellow
    dotnet clean SastImg.Client/SastImg.Client.csproj -p:Platform=$Platform
    Write-Host "✓ 清理完成" -ForegroundColor Green
    Write-Host ""
}

# 编译项目
Write-Host "正在编译项目..." -ForegroundColor Yellow
Write-Host ""

if ($Rebuild) {
    dotnet build SastImg.Client/SastImg.Client.csproj -p:Platform=$Platform --no-incremental
} else {
    dotnet build SastImg.Client/SastImg.Client.csproj -p:Platform=$Platform
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  ✓ 编译成功！" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "输出目录：" -ForegroundColor Yellow
    Write-Host "  SastImg.Client\bin\$Platform\Debug\net9.0-windows10.0.22621.0\win-$Platform\" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  ✗ 编译失败" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
}

Write-Host ""
Write-Host "使用说明：" -ForegroundColor Cyan
Write-Host "  .\build.ps1              # 增量编译 (x64)" -ForegroundColor White
Write-Host "  .\build.ps1 -Platform ARM64  # 编译 ARM64 版本" -ForegroundColor White
Write-Host "  .\build.ps1 -Clean       # 清理后编译" -ForegroundColor White
Write-Host "  .\build.ps1 -Rebuild     # 完全重新编译" -ForegroundColor White
