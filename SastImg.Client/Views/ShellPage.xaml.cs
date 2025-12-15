using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Windows.System;

namespace SastImg.Client.Views;

public sealed partial class ShellPage : Page
{
    private ShellPageViewModel vm = new();

    public ShellPage()
    {
        this.InitializeComponent();
        // Default to home page
        MainFrame.Navigate(typeof(HomeView));
        NavView.SelectedItem = NavView.MenuItems[0];

        // Listen to navigation events to update back button state
        MainFrame.Navigated += OnNavigated;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        // Update back button visibility
        // This automatically updates UI through x:Bind MainFrame.CanGoBack
    }

    private void TitleBar_BackButtonClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (MainFrame.CanGoBack)
        {
            MainFrame.GoBack();
        }
    }

    private async void NavigationView_ItemInvoked(
        NavigationView sender,
        NavigationViewItemInvokedEventArgs args
    )
    {
        if (args.InvokedItemContainer is NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "Home":
                    MainFrame.Navigate(typeof(HomeView));
                    break;
                case "Categories":
                    MainFrame.Navigate(typeof(CategoryView));
                    break;
                case "Settings":
                    MainFrame.Navigate(typeof(SettingsView));
                    break;
                case "GitHub":
                    await Launcher.LaunchUriAsync(
                        new Uri("https://github.com/NJUPT-SAST-Csharp/Winter-Of-Code-2024")
                    );
                    break;
            }
        }
    }
}
