using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SastImg.Client.Views.Dialogs;

namespace SastImg.Client.Views;
public partial class ShellPageViewModel : ObservableObject
{
    const string DefaultUsername =  "Not logged in";

    public ShellPageViewModel ( )
    {
        App.AuthService.LoginStateChanged += OnLoginStatusChanged;
    }

    public string Username => IsLoggedIn ? (App.AuthService.Username ?? DefaultUsername) : DefaultUsername;

    public bool IsLoggedIn => App.AuthService.IsLoggedIn;

    public void OnLoginStatusChanged (bool isLogin, string? username)
    {
        OnPropertyChanged(nameof(IsLoggedIn));
        OnPropertyChanged(nameof(Username));
    }

    public ICommand LoginCommand => new RelayCommand(async ( ) =>
    {
        var dialog = new LoginDialog();
        await dialog.ShowAsync();
    });

    public ICommand RegisterCommand => new RelayCommand(async ( ) =>
    {
        var dialog = new RegisterDialog();
        await dialog.ShowAsync();
    });

    public ICommand LogoutCommand => new RelayCommand(( ) =>
    {
        App.AuthService.Logout();
    });

    public ICommand ViewProfileCommand => new RelayCommand(async ( ) =>
    {
        // Get current user ID - we need to track this in AuthService
        // For now, we'll need to add a way to get the user ID
        // This is a limitation - the API doesn't return user ID on login
        // We would need to call a "GetCurrentUser" endpoint or similar
        
        // Navigate to user profile view
        if (App.Shell?.MainFrame != null)
        {
            // For now, we'll pass 0 and let the view handle getting current user info
            App.Shell.MainFrame.Navigate(typeof(UserProfileView), 0L);
        }
    });
}
