using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views.Dialogs;

public sealed partial class ChangePasswordDialog : ContentDialog
{
    public ChangePasswordDialog()
    {
        this.InitializeComponent();
    }

    private async void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            if (string.IsNullOrWhiteSpace(OldPasswordBox.Password))
            {
                ErrorInfoBar.Message = "Please enter your current password.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPasswordBox.Password) || 
                NewPasswordBox.Password.Length < 6 || 
                NewPasswordBox.Password.Length > 200)
            {
                ErrorInfoBar.Message = "New password must be between 6 and 200 characters.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ErrorInfoBar.Message = "Passwords do not match.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            var request = new ResetPasswordRequest
            {
                OldPassword = OldPasswordBox.Password,
                NewPassword = NewPasswordBox.Password
            };

            var response = await App.API.Account.ResetPasswordAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                ErrorInfoBar.Message = "Failed to update password. Please check your current password.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
            }
        }
        catch (Exception ex)
        {
            ErrorInfoBar.Message = $"Error: {ex.Message}";
            ErrorInfoBar.IsOpen = true;
            args.Cancel = true;
        }
        finally
        {
            deferral.Complete();
        }
    }
}
