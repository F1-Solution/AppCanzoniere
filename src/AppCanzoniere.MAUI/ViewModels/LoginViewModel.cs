using AppCanzoniere.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppCanzoniere.MAUI.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _isRegisterMode = false;

    [ObservableProperty]
    private bool _isBusy = false;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isAuthenticated = false;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "Please enter email and password";
            return;
        }

        IsBusy = true;
        StatusMessage = "Logging in...";

        try
        {
            var result = await _authService.LoginAsync(Email, Password);

            if (result != null)
            {
                IsAuthenticated = true;
                StatusMessage = $"Welcome {result.Email}!";

                // Navigate back or to main page
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                StatusMessage = "Login failed. Please check your credentials.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            StatusMessage = "Please enter email and password";
            return;
        }

        if (Password != ConfirmPassword)
        {
            StatusMessage = "Passwords do not match";
            return;
        }

        IsBusy = true;
        StatusMessage = "Registering...";

        try
        {
            var result = await _authService.RegisterAsync(Email, Password, ConfirmPassword);

            if (result != null)
            {
                IsAuthenticated = true;
                StatusMessage = $"Welcome {result.Email}! Registration successful.";

                // Navigate back or to main page
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                StatusMessage = "Registration failed. Please try again.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        IsAuthenticated = false;
        Email = string.Empty;
        Password = string.Empty;
        ConfirmPassword = string.Empty;
        StatusMessage = "Logged out successfully";
    }

    [RelayCommand]
    private void ToggleMode()
    {
        IsRegisterMode = !IsRegisterMode;
        StatusMessage = string.Empty;
    }

    public async Task CheckAuthenticationAsync()
    {
        IsAuthenticated = await _authService.IsAuthenticatedAsync();
    }
}
