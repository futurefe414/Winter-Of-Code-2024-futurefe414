using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SastImg.Client.Views.Dialogs;

public sealed partial class EditImageDialog : ContentDialog
{
    private readonly long _imageId;

    public EditImageDialog(long imageId, string currentTitle, List<long> currentTags)
    {
        this.InitializeComponent();
        _imageId = imageId;
        
        TitleTextBox.Text = currentTitle;
        if (currentTags != null && currentTags.Count > 0)
        {
            TagsTextBox.Text = string.Join(", ", currentTags);
        }
    }

    private async void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            // Note: The API doesn't seem to have direct endpoints to update image title/tags
            // This is a limitation of the current API design
            // We would need UpdateImageTitle and UpdateImageTags endpoints
            
            ErrorInfoBar.Message = "Image editing is not yet supported by the API.";
            ErrorInfoBar.IsOpen = true;
            args.Cancel = true;
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
