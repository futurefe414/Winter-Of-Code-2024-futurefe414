# PowerShell 终端编码设置脚本
# 用于确保终端正确显示中文

# 设置控制台输出编码为 UTF-8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# 设置代码页为 UTF-8 (65001)
chcp 65001 | Out-Null

Write-Host "✓ 终端编码已设置为 UTF-8" -ForegroundColor Green
Write-Host "✓ 现在可以正确显示中文了" -ForegroundColor Green
Write-Host ""
Write-Host "测试中文显示：你好，世界！" -ForegroundColor Cyan
