using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxiDispatcher.Data;
using TaxiDispatcher.Models;

namespace TaxiDispatcher.Services;

public static class OrderService
{
    public static async Task<List<Order>> GetOrdersAsync(string search = "", int statusId = 0, bool isDescending = true)
    {
        using var db = Db.Create();
        var query = db.Orders.Include(o => o.Status).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(o => o.ClientName.Contains(search)
                || o.ClientPhone.Contains(search)
                || o.PickupAddress.Contains(search)
                || o.DestinationAddress.Contains(search));
        }

        if (statusId > 0)
        {
            query = query.Where(o => o.StatusId == statusId);
        }

        query = isDescending ? query.OrderByDescending(o => o.OrderDate) : query.OrderBy(o => o.OrderDate);

        return await query.ToListAsync();
    }

    public static async Task SaveOrderAsync(Order order)
    {
        using var db = Db.Create();
        
        if (order.OrderId == 0) 
        {
            order.OrderDate = DateTime.Now;
            if (order.StatusId == 0) order.StatusId = 1;
            db.Orders.Add(order);
        } 
        else 
        {
            var existing = await db.Orders.FindAsync(order.OrderId);
            if (existing != null)
            {
                existing.ClientName = order.ClientName;
                existing.ClientPhone = order.ClientPhone;
                existing.PickupAddress = order.PickupAddress;
                existing.DestinationAddress = order.DestinationAddress;
                existing.DistanceKm = order.DistanceKm;
            }
        }
        await db.SaveChangesAsync();
    }
}
