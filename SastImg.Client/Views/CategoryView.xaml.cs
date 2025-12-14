using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views;

public sealed partial class CategoryView : Page
{
    public CategoryViewModel ViewModel { get; }

    public CategoryView()
    {
        ViewModel = new CategoryViewModel();
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadCategoriesAsync();
    }

    private void CategoryGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is CategoryDto category)
        {
            // Navigate to AlbumView with category ID as parameter
            App.Shell?.MainFrame.Navigate(typeof(AlbumView), category.Id);
        }
    }
}
