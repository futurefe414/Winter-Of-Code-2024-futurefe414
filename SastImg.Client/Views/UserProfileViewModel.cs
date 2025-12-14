using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace SastImg.Client.Views;

public partial class UserProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private long _userId;

    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _biography = "";

    public async Task LoadProfileAsync(long userId)
    {
        IsLoading = true;
        UserId = userId;

        try
        {
            var response = await App.API.User.GetProfileInfoAsync(userId);
            if (response.IsSuccessStatusCode && response.Content != null)
            {
                Username = response.Content.Username ?? "";
                Biography = response.Content.Biography ?? "No biography";
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
