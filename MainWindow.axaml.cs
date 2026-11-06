using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using TaxiDispatcher.Services;
using TaxiDispatcher.Views;
using TaxiDispatcher.ViewModels;

namespace TaxiDispatcher;

public partial class MainWindow : Window
{
    private int _themeState = 0;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (s, e) => UpdateThemeIconAndTooltip();
    }

    public void ShowMenu() 
    { 
        MenuPanel.IsVisible = true;
        ConfigureMenuByRole();
    }
    
    public void HideMenu() => MenuPanel.IsVisible = false;

    private void ConfigureMenuByRole()
    {
        string roleName = Session.CurrentUser?.Role?.RoleName?.Trim().ToLower() ?? "";
        bool isAdmin = roleName == "администратор";

        UsersBtn.IsVisible = isAdmin;
        TariffsBtn.IsVisible = isAdmin;
    }

    private void ThemeClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current == null) 
            return;

        _themeState = (_themeState + 1) % 3;

        if (_themeState == 0)
            Application.Current.RequestedThemeVariant = ThemeVariant.Default;
        else if (_themeState == 1)
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        else
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;

        UpdateThemeIconAndTooltip();
    }

    private void UpdateThemeIconAndTooltip()
    {
        if (ThemeIcon == null || ThemeBtn == null || Application.Current == null) 
            return;

        if (_themeState == 0)
        {
            ThemeIcon.Data = (StreamGeometry)Application.Current.FindResource("MonitorIcon")!;
            ToolTip.SetTip(ThemeBtn, "Системная тема");
        }
        else if (_themeState == 1)
        {
            ThemeIcon.Data = (StreamGeometry)Application.Current.FindResource("SunIcon")!;
            ToolTip.SetTip(ThemeBtn, "Светлая тема");
        }
        else
        {
            ThemeIcon.Data = (StreamGeometry)Application.Current.FindResource("MoonIcon")!;
            ToolTip.SetTip(ThemeBtn, "Тёмная тема");
        }
    }

    private void OrdersClick(object? sender, RoutedEventArgs e) 
        => (DataContext as MainViewModel)?.CurrentPage = new OrdersView();

    private void UsersClick(object? sender, RoutedEventArgs e) 
        => (DataContext as MainViewModel)?.CurrentPage = new UsersView();

    private void TariffsClick(object? sender, RoutedEventArgs e) 
        => (DataContext as MainViewModel)?.CurrentPage = new TariffsView();

    private void LogoutClick(object? sender, RoutedEventArgs e)
    {
        Session.Logout();
        HideMenu();

        (DataContext as MainViewModel)?.CurrentPage = new LoginView();
    }
}
