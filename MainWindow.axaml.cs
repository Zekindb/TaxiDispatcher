using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaxiDispatcher.Services;
using TaxiDispatcher.Views;
using TaxiDispatcher.ViewModels;

namespace TaxiDispatcher;

public partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; } = null!;

    private DateTime _lastToastTime = DateTime.MinValue;
    private int _themeState = 0;

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
        Loaded += (s, e) => UpdateThemeIconAndTooltip();
    }

    public async void ShowToasts(List<string> messages, bool isError = true)
    {
        if (messages == null || messages.Count == 0)
            return;

        if ((DateTime.Now - _lastToastTime).TotalSeconds < 2.5)
            return;

        _lastToastTime = DateTime.Now;

        foreach (var msg in messages)
        {
            if (ToastContainer.Children.Count >= 5)
                ToastContainer.Children.RemoveAt(0);

            var toast = new Border
            {
                Background = new SolidColorBrush(Color.Parse(isError ? "#E74C3C" : "#27AE60")),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20, 12),
                Opacity = 0,
                Margin = new Thickness(0, 0, 0, -20),
                BoxShadow = BoxShadows.Parse("0 4 12 0 #40000000"),
                Child = new TextBlock 
                { 
                    Text = msg, 
                    Foreground = Brushes.White, 
                    FontWeight = FontWeight.Bold 
                },
                Transitions = new Transitions
                {
                    new DoubleTransition { Property = OpacityProperty, Duration = TimeSpan.FromSeconds(0.3) },
                    new ThicknessTransition { Property = MarginProperty, Duration = TimeSpan.FromSeconds(0.3), Easing = new CubicEaseOut() }
                }
            };

            ToastContainer.Children.Add(toast);

            toast.Opacity = 1;
            toast.Margin = new Thickness(0);

            _ = HideToastAsync(toast);
            await Task.Delay(300);
        }
    }

    private async Task HideToastAsync(Border toast)
    {
        await Task.Delay(5000);

        toast.Opacity = 0;
        toast.Margin = new Thickness(0, 0, 0, -20);
        await Task.Delay(300);
        ToastContainer.Children.Remove(toast);
    }

    public void ShowToast(string message, bool isError = true)
    {
        ShowToasts(new List<string> { message }, isError);
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
