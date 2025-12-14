using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views.Dialogs;

public sealed partial class EditAlbumDialog : ContentDialog
{
    private readonly long _albumId;

    public EditAlbumDialog(long albumId, string currentTitle, string currentDescription, int currentAccessLevel)
    {
        this.InitializeComponent();
        _albumId = albumId;
        
        TitleTextBox.Text = currentTitle;
        DescriptionTextBox.Text = currentDescription;
        AccessLevelComboBox.SelectedIndex = currentAccessLevel;
    }

    private async void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            // Validate title
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                ErrorInfoBar.Message = "Title is required.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            // Validate description
            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text) || DescriptionTextBox.Text.Trim().Length < 3)
            {
                ErrorInfoBar.Message = "Description must be at least 3 characters.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            // Update title
            var titleRequest = new UpdateTitleRequest { Title = TitleTextBox.Text.Trim() };
            var titleResponse = await App.API.Album.UpdateAlbumTitleAsync(_albumId, titleRequest);
            
            if (!titleResponse.IsSuccessStatusCode)
            {
                ErrorInfoBar.Message = "Failed to update title.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            // Update description
            var descRequest = new UpdateDescriptionRequest { Description = DescriptionTextBox.Text.Trim() };
            var descResponse = await App.API.Album.UpdateAlbumDescriptionAsync(_albumId, descRequest);
            
            if (!descResponse.IsSuccessStatusCode)
            {
                ErrorInfoBar.Message = "Failed to update description.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            // Update access level
            var accessRequest = new UpdateAccessLevelRequest { AccessLevel = AccessLevelComboBox.SelectedIndex };
            var accessResponse = await App.API.Album.UpdateAlbumAccessLevelAsync(_albumId, accessRequest);
            
            if (!accessResponse.IsSuccessStatusCode)
            {
                ErrorInfoBar.Message = "Failed to update access level.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            args.Cancel = false;
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
