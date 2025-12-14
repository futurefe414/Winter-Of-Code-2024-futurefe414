# PowerShell 终端编码永久配置指南

## 已完成的配置

✅ PowerShell Profile 已自动配置完成！

**Profile 文件位置：**
```
D:\Users\ASUS\AppData\Local\Programs\Documents\WindowsPowerShell\Microsoft.PowerShell_profile.ps1
```

## 配置内容

Profile 文件中已添加以下内容：

```powershell
# PowerShell 启动时自动设置 UTF-8 编码
# 这样可以正确显示中文

# 设置控制台输出编码为 UTF-8
$OutputEncoding = [System.Text.Encoding]::UTF8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# 设置代码页为 UTF-8 (65001)
chcp 65001 | Out-Null
```

## 生效方式

### 方法 1：重启 PowerShell（推荐）
关闭当前 PowerShell 窗口，重新打开一个新的 PowerShell 窗口，配置将自动生效。

### 方法 2：重新加载 Profile
在当前 PowerShell 窗口中运行：
```powershell
. $PROFILE
```

### 方法 3：手动执行（临时）
如果不想重启，可以手动执行：
```powershell
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
chcp 65001
```

## 验证配置

重启 PowerShell 后，运行以下命令验证：

```powershell
# 检查输出编码
[Console]::OutputEncoding

# 检查代码页（应该显示 65001）
chcp

# 测试中文显示
Write-Host "测试中文：你好，世界！" -ForegroundColor Cyan
```

## 其他终端配置

### Windows Terminal
如果你使用 Windows Terminal，还可以在设置中配置：

1. 打开 Windows Terminal 设置（Ctrl + ,）
2. 选择你的 PowerShell 配置文件
3. 在 "命令行" 中添加：
   ```
   powershell.exe -NoExit -Command "[Console]::OutputEncoding=[System.Text.Encoding]::UTF8"
   ```

### VS Code 集成终端
在 VS Code 的 `settings.json` 中添加：

```json
{
    "terminal.integrated.shellArgs.windows": [
        "-NoExit",
        "-Command",
        "[Console]::OutputEncoding=[System.Text.Encoding]::UTF8"
    ]
}
```

## 常见问题

### Q: 为什么有时候还是显示乱码？
A: 某些程序可能会重置编码设置。可以在运行这些程序前手动执行：
```powershell
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
```

### Q: 如何查看当前 Profile 内容？
A: 运行以下命令：
```powershell
Get-Content $PROFILE
```

### Q: 如何编辑 Profile 文件？
A: 运行以下命令之一：
```powershell
# 使用记事本
notepad $PROFILE

# 使用 VS Code
code $PROFILE

# 使用 PowerShell ISE
ise $PROFILE
```

### Q: 如何禁用自动配置？
A: 编辑 Profile 文件，注释掉或删除相关行：
```powershell
notepad $PROFILE
```

## 项目编译脚本

为了方便使用，项目中已包含自动设置编码的编译脚本：

### build.ps1
```powershell
# 基本编译
.\build.ps1

# 编译 ARM64 版本
.\build.ps1 -Platform ARM64

# 清理后编译
.\build.ps1 -Clean

# 完全重新编译
.\build.ps1 -Rebuild
```

### setup-encoding.ps1
快速设置当前会话的编码：
```powershell
.\setup-encoding.ps1
```

## 系统级配置（可选）

如果想要系统级别的永久配置，可以修改注册表：

⚠️ **警告：修改注册表有风险，请谨慎操作！**

```powershell
# 以管理员身份运行 PowerShell，然后执行：
Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\Nls\CodePage" -Name "OEMCP" -Value "65001"
Set-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\Nls\CodePage" -Name "ACP" -Value "65001"
```

**注意：** 修改后需要重启计算机才能生效。

## 总结

✅ PowerShell Profile 已配置完成
✅ 下次启动 PowerShell 时将自动使用 UTF-8 编码
✅ 中文将正确显示
✅ 项目编译输出将正确显示中文

如有问题，请参考上述常见问题部分或重新运行配置脚本。
