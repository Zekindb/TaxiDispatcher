using Avalonia.Controls;
using TaxiDispatcher.Data;
using TaxiDispatcher.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            db.Database.EnsureCreated();

            if (!db.Roles.Any())
            {
                var rAdmin = new Role { RoleName = "Администратор" };
                var rDisp = new Role { RoleName = "Диспетчер" };
                var rDriver = new Role { RoleName = "Водитель" };
                var rClient = new Role { RoleName = "Клиент" };

                db.Roles.AddRange(rAdmin, rDisp, rDriver, rClient);
                db.SaveChanges();

                db.Users.AddRange(
                    new User { Login = "admin", FullName = "Сидоров Алексей", Phone = "+79991112233", RoleId = rAdmin.RoleId },
                    new User { Login = "disp1", FullName = "Иванова Мария", Phone = "+79992223344", RoleId = rDisp.RoleId },
                    new User { Login = "driver1", FullName = "Петров Иван", Phone = "+79993334455", RoleId = rDriver.RoleId },
                    new User { Login = "client1", FullName = "Козлов Артем", Phone = "+79995556677", RoleId = rClient.RoleId }
                );
                db.SaveChanges();
            }

            var users = await db.Users.Include(u => u.Role).ToListAsync();
            UsersList.ItemsSource = users.Select(u => $"{u.FullName} - {u.Role?.RoleName}").ToList();
        }
        catch (Exception ex)
        {
            UsersList.ItemsSource = new List<string> { "Ошибка: " + ex.Message + " | Детали: " + ex.InnerException?.Message };
        }
    }
}
