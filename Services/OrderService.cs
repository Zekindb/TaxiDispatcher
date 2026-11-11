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
        var query = db.Orders.Include(o => o.Status).Include(o => o.Tariff).Include(o => o.Invoice).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(o => o.ClientName.Contains(search)
                || o.ClientPhone.Contains(search)
                || o.PickupAddress.Contains(search));
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

        Tariff? tariff = null;
        if (order.TariffId.HasValue)
        {
            tariff = await db.Tariffs.FindAsync(order.TariffId.Value);
        }

        decimal finalPrice = TariffService.CalculatePrice(tariff, order.DistanceKm);

        if (order.OrderId == 0) 
        {
            order.OrderDate = DateTime.Now;
            if (order.StatusId == 0) order.StatusId = 1;
            db.Orders.Add(order);
            await db.SaveChangesAsync(); 

            if (finalPrice > 0)
            {
                db.Invoices.Add(new Invoice { OrderId = order.OrderId, Amount = finalPrice, PaymentStatus = "Не оплачен" });
                await db.SaveChangesAsync();
            }
        } 
        else 
        {
            var existing = await db.Orders.Include(o => o.Invoice).FirstOrDefaultAsync(o => o.OrderId == order.OrderId);
            if (existing != null)
            {
                existing.ClientName = order.ClientName;
                existing.ClientPhone = order.ClientPhone;
                existing.PickupAddress = order.PickupAddress;
                existing.DestinationAddress = order.DestinationAddress;
                existing.DistanceKm = order.DistanceKm;
                existing.TariffId = order.TariffId;

                if (existing.Invoice != null && finalPrice > 0)
                    existing.Invoice.Amount = finalPrice;
                else if (existing.Invoice == null && finalPrice > 0)
                    db.Invoices.Add(new Invoice { OrderId = existing.OrderId, Amount = finalPrice, PaymentStatus = "Не оплачен" });

                await db.SaveChangesAsync();
            }
        }
    }

    public static async Task<List<OrderStatus>> GetStatusesAsync()
    {
        using var db = Db.Create();
        return await db.OrderStatuses.ToListAsync();
    }

    public static async Task UpdateOrderStatusAsync(int orderId, int statusId)
    {
        using var db = Db.Create();
        var order = await db.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.StatusId = statusId;
            await db.SaveChangesAsync();
        }
    }
}
