using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views;

public partial class AlbumViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private string _categoryName = "Albums";

    [ObservableProperty]
    private long _categoryId;

    public ObservableCollection<AlbumDto> Albums { get; } = new();

    public async Task LoadAlbumsAsync(long categoryId)
    {
        if (IsLoading)
            return;

        IsLoading = true;
        CategoryId = categoryId;
        Albums.Clear();

        try
        {
            var response = await App.API!.Album.GetAlbumsAsync(categoryId, null, null);
            
            if (response.IsSuccessful && response.Content != null)
            {
                foreach (var album in response.Content)
                {
                    Albums.Add(album);
                }
            }

            // Optionally load category name
            // var categoryResponse = await App.API!.Category.GetCategoryAsync();
            // if (categoryResponse.IsSuccessful && categoryResponse.Content != null)
            // {
            //     var category = categoryResponse.Content.FirstOrDefault(c => c.Id == categoryId);
            //     if (category != null)
            //         CategoryName = category.Name;
            // }
        }
        catch
        {
            // Handle error
        }
        finally
        {
            IsLoading = false;
        }
    }
}
