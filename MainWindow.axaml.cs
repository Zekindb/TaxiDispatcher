using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Avalonia.Controls;
using TaxiDispatcher.Data;
using Microsoft.EntityFrameworkCore;

namespace TaxiDispatcher;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += async (s, e) => await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            using var db = Db.Create();
            var users = await db.Users.Include(u => u.Role).ToListAsync();
            UsersList.ItemsSource = users.Select(u => $"{u.FullName} - {u.Role?.RoleName}").ToList();
        }
        catch (Exception ex)
        {
            UsersList.ItemsSource = new List<string> { "Ошибка подключения к БД: " + ex.Message };
        }
    }
}
