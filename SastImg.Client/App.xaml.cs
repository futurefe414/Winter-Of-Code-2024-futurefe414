using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using SastImg.Client.Helpers;
using SastImg.Client.Services;
using SastImg.Client.Views;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SastImg.Client;

public partial class App : Application
{
    public App ( )
    {
        this.InitializeComponent();
        
        // 使用正确的端口5265
        var apiEndpoint = "http://sastwoc2024.shirasagi.space:5265/";
        Debug.WriteLine($"[App] 初始化应用程序，API端点: {apiEndpoint}");
        
        API = new SastImgAPI(apiEndpoint);
    }

    protected override void OnLaunched (Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        Debug.WriteLine("[App] 应用程序启动");
        
        Shell = new ShellPage();
        MainWindow = new Window()
        {
            SystemBackdrop = new MicaBackdrop(),
            Title = "SAST Image",
            Content = Shell
        };
        MainWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        MainWindow.AppWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
        MainWindow.Activate();
        WindowHelper.TrackWindow(MainWindow);
        
        Debug.WriteLine("[App] 应用程序启动完成");
    }

    public static ShellPage? Shell;
    public static Window? MainWindow;
    public static SastImgAPI? API;
    public static AuthService AuthService = new();
}
