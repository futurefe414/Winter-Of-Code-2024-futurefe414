# 登录功能修复说明

## 修复的问题

### 1. 密码输入框问题
**问题**：LoginDialog 中使用 `TextBox` 显示密码，导致密码明文可见
**修复**：将密码输入框改为 `PasswordBox`

```xaml
<!-- 修复前 -->
<TextBox Text="{x:Bind Password, Mode=TwoWay}" PlaceholderText="密码" />

<!-- 修复后 -->
<PasswordBox Password="{x:Bind Password, Mode=TwoWay}" PlaceholderText="密码" />
```

### 2. 登录失败处理问题
**问题**：登录失败时对话框仍然关闭，用户无法看到错误信息
**修复**：登录失败时阻止对话框关闭，显示错误提示

```csharp
// 修复前：无论成功失败都关闭对话框
if (await App.AuthService.LoginAsync(Username, Password))
{
    // 成功后显示对话框
}
else
{
    // 失败后也显示对话框（但主对话框已关闭）
}

// 修复后：失败时保持对话框打开
bool loginSuccess = await App.AuthService.LoginAsync(Username, Password);

if (loginSuccess)
{
    args.Cancel = false;  // 允许关闭
}
else
{
    args.Cancel = true;   // 阻止关闭
    IsLoginFailed = true; // 显示错误信息
}
```

### 3. API 端点配置问题
**问题**：`SastImgAPI` 构造函数接收 `endpointUrl` 参数但未使用，导致硬编码的 URL 与配置不一致
**修复**：使用传入的 `endpointUrl` 参数初始化所有 API 接口

```csharp
// 修复前：忽略 endpointUrl 参数
public SastImgAPI(string endpointUrl)
{
    Account = RestService.For<IAccountApi>("http://sastwoc2024.shirasagi.space:5265/", refitSettings);
    // ...
}

// 修复后：使用 endpointUrl 参数
public SastImgAPI(string endpointUrl)
{
    Account = RestService.For<IAccountApi>(endpointUrl, refitSettings);
    Image = RestService.For<IImageApi>(endpointUrl, refitSettings);
    Album = RestService.For<IAlbumApi>(endpointUrl, refitSettings);
    Category = RestService.For<ICategoryApi>(endpointUrl, refitSettings);
    Tag = RestService.For<ITagApi>(endpointUrl, refitSettings);
    User = RestService.For<IUserApi>(endpointUrl, refitSettings);
}
```

### 4. API URL 端口不一致
**问题**：`App.xaml.cs` 中使用端口 5263，但原代码硬编码使用 5265
**修复**：统一使用正确的端口 5265

```csharp
// App.xaml.cs
API = new SastImgAPI("http://sastwoc2024.shirasagi.space:5265/");
```

## 测试建议

1. **测试登录成功场景**
   - 使用正确的用户名和密码（test/123456 或 admin/123456）
   - 验证登录成功后对话框关闭
   - 验证右上角显示用户名

2. **测试登录失败场景**
   - 使用错误的用户名或密码
   - 验证对话框保持打开状态
   - 验证显示错误提示信息
   - 验证可以重新输入并再次尝试

3. **测试密码安全性**
   - 验证密码输入时显示为圆点或星号
   - 验证密码不会明文显示

## 相关文件

- `SastImg.Client/Views/Dialogs/LoginDialog.xaml` - 登录对话框 UI
- `SastImg.Client/Views/Dialogs/LoginDialog.xaml.cs` - 登录对话框逻辑
- `SastImg.Client/Services/SastImgAPI.cs` - API 服务配置
- `SastImg.Client/App.xaml.cs` - 应用初始化
- `SastImg.Client/Services/AuthService.cs` - 认证服务

## 编译状态

✅ 项目编译成功（0 个错误，77 个警告）

使用以下命令编译：
```bash
dotnet build SastImg.Client/SastImg.Client.csproj -p:Platform=x64
```
