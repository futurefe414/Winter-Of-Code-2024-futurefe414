using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views;

public partial class ImageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ImageDto> images = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string albumTitle = "Images";

    [ObservableProperty]
    private string albumDescription = "";

    [ObservableProperty]
    private int albumAccessLevel = 0;

    public long AlbumId { get; private set; }

    public async Task LoadImagesAsync(long albumId)
    {
        AlbumId = albumId;
        IsLoading = true;

        try
        {
            // Load album details
            var albumResponse = await App.API.Album.GetDetailedAlbumAsync(albumId);
            if (albumResponse.IsSuccessStatusCode && albumResponse.Content != null)
            {
                AlbumTitle = albumResponse.Content.Title ?? "Images";
                AlbumDescription = albumResponse.Content.Description ?? "";
                AlbumAccessLevel = albumResponse.Content.AccessLevel;
            }

            // Load images
            var response = await App.API.Image.GetImagesAsync(null, albumId, null);
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                Images.Clear();
                foreach (var image in response.Content)
                {
                    Images.Add(image);
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
