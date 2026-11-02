using System.Threading.Tasks;
using TaxiDispatcher.Data;
using TaxiDispatcher.Models;
using Microsoft.EntityFrameworkCore;

namespace TaxiDispatcher.Services;

public static class Session
{
    public static User? CurrentUser { get; private set; }
    public static string Caption => CurrentUser == null ? "Гость" : $"{CurrentUser.FullName} ({CurrentUser.Role?.RoleName})";

    public static async Task LoginAsync(string login)
    {
        using var db = Db.Create();
        CurrentUser = await db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == login);
    }

    public static void Logout() => CurrentUser = null;
}
