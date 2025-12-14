# 登录对话框编码问题和异常处理修复

## 问题描述

### 1. 编码问题
登录对话框（LoginDialog.xaml）中的中文字符显示为乱码，例如：
- "鐧诲綍" 应该是 "登录"
- "鍙栨秷" 应该是 "取消"
- "鐢ㄦ埛鍚?" 应该是 "用户名"
- "瀵嗙爜" 应该是 "密码"

### 2. TaskCanceledException 异常
当用户点击取消按钮时，会抛出 `TaskCanceledException` 异常，虽然不影响功能但会在调试输出中显示。

## 根本原因

### 编码问题
XAML 文件的编码出现问题，导致中文字符被错误解析。

### 异常问题
登录对话框没有正确处理取消操作，导致 `CancellationToken` 被触发时抛出未捕获的异常。

## 修复方案

### 编码修复
重新创建 `LoginDialog.xaml` 文件，使用正确的 UTF-8 BOM 编码，并确保所有中文字符正确显示。

### 异常处理修复
1. 在 `LoginDialog.xaml.cs` 中添加 `OperationCanceledException` 的捕获处理
2. 在 `AuthService.cs` 中添加 `CancellationToken` 参数支持
3. 正确处理取消操作，避免异常传播到调试输出

## 修复内容

### 修改的文件
- `SastImg.Client/Views/Dialogs/LoginDialog.xaml` - 修复编码问题
- `SastImg.Client/Views/Dialogs/LoginDialog.xaml.cs` - 添加取消异常处理
- `SastImg.Client/Services/AuthService.cs` - 添加 CancellationToken 支持

### 正确的中文文本
- Title: "登录"
- CloseButtonText: "取消"
- PrimaryButtonText: "登录"
- Username PlaceholderText: "用户名"
- Password PlaceholderText: "密码"
- Error Message: "登录失败，请检查用户名和密码"

### 异常处理改进
1. **LoginDialog.xaml.cs**:
   - 添加 `OperationCanceledException` 捕获块
   - 取消操作时允许对话框正常关闭
   - 传递 `CancellationToken` 到 `AuthService.LoginAsync`

2. **AuthService.cs**:
   - 添加 `CancellationToken` 参数（默认值为 `default`）
   - 将 token 传递给 API 调用
   - 单独捕获 `TaskCanceledException` 和 `OperationCanceledException`
   - 重新抛出取消异常以便调用者处理
   - 添加 `using System.Threading;` 引用

## 测试说明

### 编译测试
```powershell
chcp 65001
dotnet build SastImg.Client/SastImg.Client.csproj -p:Platform=x64
```
编译成功，0 个错误，77 个警告（警告为预期的 AOT 兼容性警告）。

### 功能测试
1. 运行应用程序
2. 点击登录按钮打开登录对话框
3. 验证所有中文文本正确显示
4. 使用测试账号登录：
   - test/123456
   - admin/123456

### 调试日志
登录过程中会输出详细的调试信息：
- `LoginDialog.xaml.cs`: 显示登录尝试和结果
- `AuthService.cs`: 显示 API 请求状态、HTTP 状态码、Token 信息等

## 相关文件
- `SastImg.Client/Views/Dialogs/LoginDialog.xaml` - 登录对话框 UI
- `SastImg.Client/Views/Dialogs/LoginDialog.xaml.cs` - 登录对话框逻辑
- `SastImg.Client/Services/AuthService.cs` - 认证服务（包含详细日志）
- `SastImg.Client/Services/SastImgAPI.cs` - API 客户端
- `SastImg.Client/App.xaml.cs` - 应用程序入口（API 端点配置）

## API 配置
- 端点: `http://sastwoc2024.shirasagi.space:5265/`
- 测试账号: test/123456, admin/123456

## 注意事项
1. 确保 PowerShell 使用 UTF-8 编码（`chcp 65001`）
2. 如果登录失败，检查调试输出中的详细错误信息
3. 确认 API 端点可访问
4. 验证网络连接正常
