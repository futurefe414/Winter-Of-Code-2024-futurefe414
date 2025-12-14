using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views.Dialogs;

public sealed partial class CreateAlbumDialog : ContentDialog
{
    private readonly long _categoryId;

    public CreateAlbumDialog(long categoryId)
    {
        this.InitializeComponent();
        _categoryId = categoryId;
    }

    private async void CreateButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
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

            if (TitleTextBox.Text.Length > 200)
            {
                ErrorInfoBar.Message = "Title must be 200 characters or less.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            // Validate description
            var description = string.IsNullOrWhiteSpace(DescriptionTextBox.Text) 
                ? "No description" 
                : DescriptionTextBox.Text.Trim();

            if (description.Length < 3)
            {
                ErrorInfoBar.Message = "Description must be at least 3 characters.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            // Create album request
            var request = new CreateAlbumRequest
            {
                Title = TitleTextBox.Text.Trim(),
                Description = description,
                AccessLevel = AccessLevelComboBox.SelectedIndex,
                CategoryId = _categoryId
            };

            var response = await App.API.Album.CreateAlbumAsync(request);

            if (response.IsSuccessStatusCode)
            {
                args.Cancel = false;
            }
            else
            {
                ErrorInfoBar.Message = $"Failed to create album: {response.Error?.Content}";
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
