using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Refit;

namespace SastImg.Client.Views;

public sealed partial class UserProfileView : Page
{
    public UserProfileViewModel ViewModel { get; }

    public UserProfileView()
    {
        this.InitializeComponent();
        ViewModel = new UserProfileViewModel();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        long userId = 0;
        if (e.Parameter is long id)
        {
            userId = id;
        }
        
        await ViewModel.LoadProfileAsync(userId);
        await LoadAvatarAsync(userId);
    }

    private async System.Threading.Tasks.Task LoadAvatarAsync(long userId)
    {
        try
        {
            var response = await App.API.User.GetAvatarAsync(userId);
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                // Note: The API returns FileStreamResult, we need to handle it properly
                // For now, we'll use a placeholder
            }
        }
        catch
        {
            // Handle error
        }
    }

    private async void ChangeAvatar_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            try
            {
                using var stream = await file.OpenStreamForReadAsync();
                var streamPart = new StreamPart(stream, file.Name, file.ContentType);
                var response = await App.API.User.UpdateAvatarAsync(streamPart);
                
                if (response.IsSuccessStatusCode)
                {
                    await LoadAvatarAsync(ViewModel.UserId);
                }
            }
            catch (Exception ex)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to update avatar: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };
                await dialog.ShowAsync();
            }
        }
    }

    private async void ChangeUsername_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Dialogs.ChangeUsernameDialog(ViewModel.Username)
        {
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.LoadProfileAsync(ViewModel.UserId);
        }
    }

    private async void EditBiography_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Dialogs.EditBiographyDialog(ViewModel.Biography)
        {
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.LoadProfileAsync(ViewModel.UserId);
        }
    }

    private async void ChangePassword_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Dialogs.ChangePasswordDialog()
        {
            XamlRoot = this.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
