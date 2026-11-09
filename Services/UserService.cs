using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxiDispatcher.Data;
using TaxiDispatcher.Models;

namespace TaxiDispatcher.Services;

public static class UserService
{
    public static async Task<List<User>> GetUsersAsync()
    {
        using var db = Db.Create();
        return await db.Users.Include(u => u.Role).ToListAsync();
    }

    public static async Task<List<Role>> GetRolesAsync()
    {
        using var db = Db.Create();
        return await db.Roles.ToListAsync();
    }

    public static async Task SaveUserAsync(User user, string plainPassword)
    {
        if (!Session.IsAdmin) throw new Exception("Нет прав на изменение пользователей.");
        
        using var db = Db.Create();
        if (user.UserId == 0) 
        {
            user.PasswordHash = AuthService.HashPassword(plainPassword);
            db.Users.Add(user);
        }
        else 
        {
            var existing = await db.Users.FindAsync(user.UserId);
            if (existing != null)
            {
                existing.LastName = user.LastName;
                existing.FirstName = user.FirstName;
                existing.MiddleName = user.MiddleName;
                
                existing.Login = user.Login;
                existing.Phone = user.Phone;
                existing.RoleId = user.RoleId;
                
                if (!string.IsNullOrWhiteSpace(plainPassword))
                    existing.PasswordHash = AuthService.HashPassword(plainPassword);
            }
        }
        await db.SaveChangesAsync();
    }
}
