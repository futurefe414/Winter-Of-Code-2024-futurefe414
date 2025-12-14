using System;
using System.Diagnostics;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace SastImg.Client.Views.Dialogs;

[ObservableObject]
public sealed partial class LoginDialog : ContentDialog
{
    [ObservableProperty]
    private string _username="";

    [ObservableProperty]
    private string _password="";

    [ObservableProperty]
    private bool _isLoggingIn=false;

    [ObservableProperty]
    private bool _isLoginFailed = false;

    [ObservableProperty]
    private string _errorMessage = "";

    private CancellationTokenSource? _loginCts;

    public LoginDialog ( )
    {
        XamlRoot = App.MainWindow?.Content.XamlRoot;
        this.InitializeComponent();
        this.PrimaryButtonClick += LoginDialog_PrimaryButtonClick;
        this.CloseButtonClick += LoginDialog_CloseButtonClick;
    }

    private void LoginDialog_CloseButtonClick (ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        _loginCts?.Cancel();
    }

    private async void LoginDialog_PrimaryButtonClick (ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        _loginCts = new();

        IsLoggingIn = true;
        IsLoginFailed = false;
        ErrorMessage = "";
        
        Debug.WriteLine($"[LoginDialog] 开始登录，用户名: {Username}");
        
        try
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "请输入用户名";
                IsLoginFailed = true;
                args.Cancel = true;
                IsLoggingIn = false;
                deferral.Complete();
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "请输入密码";
                IsLoginFailed = true;
                args.Cancel = true;
                IsLoggingIn = false;
                deferral.Complete();
                return;
            }

            bool loginSuccess = await App.AuthService.LoginAsync(Username, Password);
            
            if (loginSuccess)
            {
                Debug.WriteLine("[LoginDialog] 登录成功");
                // 登录成功，允许对话框关闭
                IsLoggingIn = false;
            }
            else
            {
                Debug.WriteLine("[LoginDialog] 登录失败");
                // 登录失败，取消对话框关闭，显示错误信息
                ErrorMessage = "登录失败，请检查用户名和密码";
                args.Cancel = true;
                IsLoggingIn = false;
                IsLoginFailed = true;
            }
        }
        catch (System.Exception ex)
        {
            Debug.WriteLine($"[LoginDialog] 登录异常: {ex.Message}");
            // 发生异常，取消对话框关闭，显示错误信息
            ErrorMessage = $"登录错误: {ex.Message}";
            args.Cancel = true;
            IsLoggingIn = false;
            IsLoginFailed = true;
        }
        finally
        {
            deferral.Complete();
        }
    }
}
