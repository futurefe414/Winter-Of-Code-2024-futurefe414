using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views.Dialogs;

public sealed partial class ChangeUsernameDialog : ContentDialog
{
    public ChangeUsernameDialog(string currentUsername)
    {
        this.InitializeComponent();
        UsernameTextBox.Text = currentUsername;
    }

    private async void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || 
                UsernameTextBox.Text.Length < 2 || 
                UsernameTextBox.Text.Length > 160)
            {
                ErrorInfoBar.Message = "Username must be between 2 and 160 characters.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            var request = new ResetUsernameRequest
            {
                Username = UsernameTextBox.Text.Trim()
            };

            var response = await App.API.Account.ResetUsernameAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                ErrorInfoBar.Message = "Failed to update username. It may already be taken.";
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
