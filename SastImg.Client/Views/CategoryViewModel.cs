using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views;

public partial class CategoryViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading = false;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    public async Task LoadCategoriesAsync()
    {
        if (IsLoading)
            return;

        IsLoading = true;
        Categories.Clear();

        try
        {
            var response = await App.API!.Category.GetCategoryAsync();
            
            if (response.IsSuccessful && response.Content != null)
            {
                foreach (var category in response.Content)
                {
                    Categories.Add(category);
                }
            }
        }
        catch
        {
            // Handle error - could show a dialog or error message
        }
        finally
        {
            IsLoading = false;
        }
    }
}
