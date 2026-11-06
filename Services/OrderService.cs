using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxiDispatcher.Data;
using TaxiDispatcher.Models;

namespace TaxiDispatcher.Services;

public static class OrderService
{
    public static async Task<List<Order>> GetOrdersAsync()
    {
        using var db = Db.Create();

        return await db.Orders
            .Include(o => o.Status)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }
}
