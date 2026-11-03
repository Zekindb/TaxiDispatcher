using Avalonia.Controls;
using Avalonia.Interactivity;
using TaxiDispatcher.Data;
using TaxiDispatcher.Models;
using TaxiDispatcher.Services;
using TaxiDispatcher.Views;
using TaxiDispatcher.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TaxiDispatcher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        try
        {
            using var db = Db.Create();
            db.Database.ExecuteSqlRaw("IF COL_LENGTH('Users', 'password_hash') IS NULL ALTER TABLE Users ADD password_hash nvarchar(255) NOT NULL DEFAULT '';");

            var admin = db.Users.FirstOrDefault(u => u.Login == "admin");
            if (admin != null && string.IsNullOrEmpty(admin.PasswordHash))
            {
                admin.PasswordHash = AuthService.HashPassword("admin123");
                db.SaveChanges();
            }
        }
        catch { /* Игнорируем при отсутствии БД */ }
    }

    public void ShowMenu() => MenuPanel.IsVisible = true;
    public void HideMenu() => MenuPanel.IsVisible = false;

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
