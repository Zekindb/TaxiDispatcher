using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaxiDispatcher.Data;
using TaxiDispatcher.Models;

namespace TaxiDispatcher.Services;

public static class AuthService
{
    public static async Task<User?> LoginAsync(string login, string password)
    {
        using var db = Db.Create();
        var user = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == login);
        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            return null;

        return await Task.Run(() => BC.Verify(password, user.PasswordHash)) ? user : null;
    }

    public static string HashPassword(string password) => BC.HashPassword(password);
}
