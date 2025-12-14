# 测试 API 连接和登录功能
$apiUrl = "http://sastwoc2024.shirasagi.space:5265"

Write-Host "测试 API 连接..." -ForegroundColor Cyan

# 测试 1: 检查 API 是否可访问
try {
    Write-Host "`n1. 测试 API 端点是否可访问..." -ForegroundColor Yellow
    $response = Invoke-WebRequest -Uri "$apiUrl/api/account/login" -Method POST -ContentType "application/json" -Body '{"username":"test","password":"invalid"}' -UseBasicParsing -ErrorAction Stop
    Write-Host "   API 可访问，状态码: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "   API 访问失败: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   状态码: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Red
}

# 测试 2: 使用 test/123456 登录
try {
    Write-Host "`n2. 测试账号 test/123456..." -ForegroundColor Yellow
    $body = @{
        username = "test"
        password = "123456"
    } | ConvertTo-Json
    
    Write-Host "   请求体: $body" -ForegroundColor Gray
    
    $response = Invoke-RestMethod -Uri "$apiUrl/api/account/login" -Method POST -ContentType "application/json; charset=utf-8" -Body $body -ErrorAction Stop
    Write-Host "   登录成功!" -ForegroundColor Green
    Write-Host "   Token: $($response.token.Substring(0, [Math]::Min(50, $response.token.Length)))..." -ForegroundColor Green
} catch {
    Write-Host "   登录失败: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   错误详情: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

# 测试 3: 使用 admin/123456 登录
try {
    Write-Host "`n3. 测试账号 admin/123456..." -ForegroundColor Yellow
    $body = @{
        username = "admin"
        password = "123456"
    } | ConvertTo-Json
    
    Write-Host "   请求体: $body" -ForegroundColor Gray
    
    $response = Invoke-RestMethod -Uri "$apiUrl/api/account/login" -Method POST -ContentType "application/json; charset=utf-8" -Body $body -ErrorAction Stop
    Write-Host "   登录成功!" -ForegroundColor Green
    Write-Host "   Token: $($response.token.Substring(0, [Math]::Min(50, $response.token.Length)))..." -ForegroundColor Green
} catch {
    Write-Host "   登录失败: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails.Message) {
        Write-Host "   错误详情: $($_.ErrorDetails.Message)" -ForegroundColor Red
    }
}

Write-Host "`n测试完成。" -ForegroundColor Cyan
