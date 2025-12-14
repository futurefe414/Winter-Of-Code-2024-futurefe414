using System;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views.Dialogs;

[ObservableObject]
public sealed partial class RegisterDialog : ContentDialog
{
    [ObservableProperty]
    private string _username = "";

    [ObservableProperty]
    private string _password = "";

    [ObservableProperty]
    private string _confirmPassword = "";

    [ObservableProperty]
    private string _registrationCode = "";

    [ObservableProperty]
    private bool _isRegistering = false;

    [ObservableProperty]
    private bool _isError = false;

    [ObservableProperty]
    private string _errorMessage = "";

    private CancellationTokenSource? _registerCts;

    public RegisterDialog()
    {
        XamlRoot = App.MainWindow?.Content.XamlRoot;
        this.InitializeComponent();
        this.PrimaryButtonClick += RegisterDialog_PrimaryButtonClick;
        this.CloseButtonClick += RegisterDialog_CloseButtonClick;
    }

    private void RegisterDialog_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        _registerCts?.Cancel();
    }

    private async void RegisterDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        _registerCts = new();

        // Validate inputs
        if (!ValidateInputs())
        {
            args.Cancel = true;
            deferral.Complete();
            return;
        }

        IsRegistering = true;
        IsError = false;
        ErrorMessage = "";

        try
        {
            // Parse registration code
            if (!int.TryParse(RegistrationCode, out int code))
            {
                IsError = true;
                ErrorMessage = "Invalid registration code format";
                args.Cancel = true;
                IsRegistering = false;
                deferral.Complete();
                return;
            }

            // Create registration request
            var request = new RegisterRequest
            {
                Username = Username,
                Password = Password,
                Code = code
            };

            // Call registration API
            var result = await App.API!.Account.RegisterAsync(request);

            if (result.IsSuccessStatusCode)
            {
                // Registration successful, now login automatically
                var loginSuccess = await App.AuthService.LoginAsync(Username, Password);

                if (loginSuccess)
                {
                    // Show success dialog after closing
                    this.Closed += (ContentDialog sender, ContentDialogClosedEventArgs args) =>
                    {
                        if (args.Result is not ContentDialogResult.Primary)
                            return;
                        var successDialog = new ContentDialog()
                        {
                            XamlRoot = this.XamlRoot,
                            Title = "Registration Successful",
                            Content = "Your account has been created and you are now logged in.",
                            CloseButtonText = "OK"
                        };
                        var _ = successDialog.ShowAsync();
                    };
                }
                else
                {
                    // Registration succeeded but auto-login failed
                    this.Closed += (ContentDialog sender, ContentDialogClosedEventArgs args) =>
                    {
                        if (args.Result is not ContentDialogResult.Primary)
                            return;
                        var successDialog = new ContentDialog()
                        {
                            XamlRoot = this.XamlRoot,
                            Title = "Registration Successful",
                            Content = "Your account has been created. Please login manually.",
                            CloseButtonText = "OK"
                        };
                        var _ = successDialog.ShowAsync();
                    };
                }
            }
            else
            {
                // Registration failed
                IsError = true;
                ErrorMessage = "Registration failed. Please check your information and try again.";
                args.Cancel = true;
            }
        }
        catch (Exception ex)
        {
            IsError = true;
            ErrorMessage = $"Error: {ex.Message}";
            args.Cancel = true;
        }
        finally
        {
            IsRegistering = false;
        }

        deferral.Complete();
    }

    private bool ValidateInputs()
    {
        // Validate username length
        if (string.IsNullOrWhiteSpace(Username) || Username.Length < 2 || Username.Length > 160)
        {
            IsError = true;
            ErrorMessage = "Username must be between 2 and 160 characters";
            return false;
        }

        // Validate password length
        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6 || Password.Length > 200)
        {
            IsError = true;
            ErrorMessage = "Password must be between 6 and 200 characters";
            return false;
        }

        // Validate password confirmation
        if (Password != ConfirmPassword)
        {
            IsError = true;
            ErrorMessage = "Passwords do not match";
            return false;
        }

        // Validate registration code
        if (string.IsNullOrWhiteSpace(RegistrationCode) || RegistrationCode.Length != 6)
        {
            IsError = true;
            ErrorMessage = "Registration code must be 6 digits";
            return false;
        }

        if (!int.TryParse(RegistrationCode, out int code) || code < 100000 || code > 999999)
        {
            IsError = true;
            ErrorMessage = "Registration code must be a 6-digit number";
            return false;
        }

        return true;
    }
}
