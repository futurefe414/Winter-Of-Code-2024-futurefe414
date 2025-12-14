using Microsoft.UI.Xaml.Controls;

namespace SastImg.Client.Views;

public sealed partial class SettingsView : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsView()
    {
        ViewModel = new SettingsViewModel();
        this.InitializeComponent();
    }
}
