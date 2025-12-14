using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SastImg.Client.Service.API;
using SastImg.Client.Views.Dialogs;

namespace SastImg.Client.Views;

public sealed partial class ImageView : Page
{
    public ImageViewModel ViewModel { get; }

    public ImageView()
    {
        this.InitializeComponent();
        ViewModel = new ImageViewModel();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is long albumId)
        {
            await ViewModel.LoadImagesAsync(albumId);
        }
    }

    private async void UploadImages_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new UploadImageDialog(ViewModel.AlbumId)
        {
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (dialog.UploadSuccessful)
        {
            await ViewModel.LoadImagesAsync(ViewModel.AlbumId);
        }
    }

    private void ImageGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is ImageDto image)
        {
            // TODO: Navigate to detailed image view
        }
    }

    public static string GetThumbnailUrl(long imageId)
    {
        return $"http://sastwoc2024.shirasagi.space:5265/api/images/{imageId}?kind=1";
    }
}
