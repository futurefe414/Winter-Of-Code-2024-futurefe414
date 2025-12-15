using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SastImg.Client.Views.Dialogs;

[ObservableObject]
public sealed partial class LoginDialog : ContentDialog
{
    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private bool _isLoggingIn = false;

    [ObservableProperty]
    private bool _isLoginFailed = false;

    private CancellationTokenSource? _loginCts;

    public LoginDialog()
    {
        XamlRoot = App.MainWindow?.Content.XamlRoot;
        this.InitializeComponent();
        this.PrimaryButtonClick += LoginDialog_PrimaryButtonClick;
        this.CloseButtonClick += LoginDialog_CloseButtonClick;
    }

    private void LoginDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        _loginCts?.Cancel();
    }

    private async void LoginDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        _loginCts = new();

        // 验证输入
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            IsLoginFailed = true;
            args.Cancel = true;
            deferral.Complete();
            return;
        }

        IsLoggingIn = true;
        IsLoginFailed = false;

        try
        {
            System.Diagnostics.Debug.WriteLine($"尝试登录: 用户名={Username}");

            bool loginSuccess = await App.AuthService.LoginAsync(Username, Password, _loginCts.Token);

            System.Diagnostics.Debug.WriteLine($"登录结果: {(loginSuccess ? "成功" : "失败")}");

            if (loginSuccess)
            {
                // 登录成功，允许对话框关闭
                args.Cancel = false;
            }
            else
            {
                // 登录失败，阻止对话框关闭并显示错误
                args.Cancel = true;
                IsLoginFailed = true;
            }
        }
        catch (System.OperationCanceledException)
        {
            // 用户取消操作，允许对话框关闭
            System.Diagnostics.Debug.WriteLine("登录操作已取消");
            args.Cancel = false;
        }
        catch (System.Exception ex)
        {
            // 发生异常，阻止对话框关闭并显示错误
            System.Diagnostics.Debug.WriteLine($"登录异常: {ex.Message}");
            args.Cancel = true;
            IsLoginFailed = true;
        }
        finally
        {
            IsLoggingIn = false;
            deferral.Complete();
        }
    }

}
