using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views;

public sealed partial class AlbumView : Page
{
    public AlbumViewModel ViewModel { get; }

    public AlbumView()
    {
        ViewModel = new AlbumViewModel();
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        
        if (e.Parameter is long categoryId)
        {
            await ViewModel.LoadAlbumsAsync(categoryId);
        }
    }

    private void AlbumGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is AlbumDto album)
        {
            Frame.Navigate(typeof(ImageView), album.Id);
        }
    }

    private async void CreateAlbum_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var dialog = new Dialogs.CreateAlbumDialog(ViewModel.CategoryId)
        {
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.LoadAlbumsAsync(ViewModel.CategoryId);
        }
    }
}
