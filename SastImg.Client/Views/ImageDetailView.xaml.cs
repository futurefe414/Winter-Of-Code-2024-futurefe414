using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.IO;

namespace SastImg.Client.Views;

public sealed partial class ImageDetailView : Page
{
    public ImageDetailViewModel ViewModel { get; }

    public ImageDetailView()
    {
        this.InitializeComponent();
        ViewModel = new ImageDetailViewModel();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is (long imageId, long albumId))
        {
            await ViewModel.LoadImageAsync(imageId, albumId);
            await LoadImageDisplayAsync(imageId);
        }
    }

    private async System.Threading.Tasks.Task LoadImageDisplayAsync(long imageId)
    {
        try
        {
            var response = await App.API.Image.GetImageAsync(imageId, 0); // 0 = Original
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                using var stream = response.Content;
                using var memStream = new MemoryStream();
                await stream.CopyToAsync(memStream);
                memStream.Position = 0;

                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(memStream.AsRandomAccessStream());
                ImageDisplay.Source = bitmap;
            }
        }
        catch
        {
            // Handle error
        }
    }

    private async void LikeButton_Click(object sender, RoutedEventArgs e)
    {
        await ViewModel.ToggleLikeAsync();
    }

    private async void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Dialogs.EditImageDialog(ViewModel.ImageId, ViewModel.ImageTitle, ViewModel.Tags)
        {
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.LoadImageAsync(ViewModel.ImageId, ViewModel.AlbumId);
        }
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Delete Image",
            Content = "Are you sure you want to delete this image? This action cannot be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var deleteResult = await ViewModel.DeleteImageAsync();
            if (deleteResult)
            {
                Frame.GoBack();
            }
        }
    }
}
