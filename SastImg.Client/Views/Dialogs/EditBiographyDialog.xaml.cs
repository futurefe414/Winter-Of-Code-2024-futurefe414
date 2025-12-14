using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views.Dialogs;

public sealed partial class EditBiographyDialog : ContentDialog
{
    public EditBiographyDialog(string currentBiography)
    {
        this.InitializeComponent();
        BiographyTextBox.Text = currentBiography;
    }

    private async void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            var request = new UpdateBiographyRequest
            {
                Biography = BiographyTextBox.Text ?? ""
            };

            var response = await App.API.User.UpdateBiographyAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                ErrorInfoBar.Message = "Failed to update biography.";
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
