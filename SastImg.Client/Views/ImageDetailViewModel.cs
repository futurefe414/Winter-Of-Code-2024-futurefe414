using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views;

public partial class ImageDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private long _imageId;

    [ObservableProperty]
    private long _albumId;

    [ObservableProperty]
    private string _imageTitle = "";

    [ObservableProperty]
    private string _uploadedAt = "";

    [ObservableProperty]
    private int _likes;

    [ObservableProperty]
    private bool _isLiked;

    [ObservableProperty]
    private string _tagsText = "No tags";

    public List<long> Tags { get; private set; } = new();

    public async Task LoadImageAsync(long imageId, long albumId)
    {
        IsLoading = true;
        ImageId = imageId;
        AlbumId = albumId;

        try
        {
            var response = await App.API.Image.GetDetailedImageAsync(imageId);
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                var image = response.Content;
                ImageTitle = image.Title ?? "Untitled";
                UploadedAt = image.UploadedAt.ToString("yyyy-MM-dd HH:mm:ss");
                Likes = image.Likes;
                IsLiked = image.Requester?.Liked ?? false;
                
                if (image.Tags != null && image.Tags.Count > 0)
                {
                    Tags = image.Tags.ToList();
                    TagsText = string.Join(", ", Tags);
                }
                else
                {
                    Tags = new List<long>();
                    TagsText = "No tags";
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task ToggleLikeAsync()
    {
        try
        {
            if (IsLiked)
            {
                var response = await App.API.Image.UnlikeImageAsync(AlbumId, ImageId);
                if (response.IsSuccessStatusCode)
                {
                    IsLiked = false;
                    Likes--;
                }
            }
            else
            {
                var response = await App.API.Image.LikeImageAsync(AlbumId, ImageId);
                if (response.IsSuccessStatusCode)
                {
                    IsLiked = true;
                    Likes++;
                }
            }
        }
        catch
        {
            // Handle error
        }
    }

    public async Task<bool> DeleteImageAsync()
    {
        try
        {
            var response = await App.API.Image.RemoveImageAsync(AlbumId, ImageId);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
