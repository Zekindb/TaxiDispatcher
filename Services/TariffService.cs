using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaxiDispatcher.Data;
using TaxiDispatcher.Models;

namespace TaxiDispatcher.Services;

public static class TariffService
{
    public static async Task<List<Tariff>> GetTariffsAsync()
    {
        using var db = Db.Create();
        return await db.Tariffs.Include(t => t.CarClass).ToListAsync();
    }

    public static async Task SaveTariffAsync(Tariff tariff)
    {
        if (!Session.IsAdmin) throw new Exception("Нет прав на изменение тарифов.");

        using var db = Db.Create();
        if (tariff.TariffId == 0) db.Tariffs.Add(tariff);
        else db.Tariffs.Update(tariff);

        await db.SaveChangesAsync();
    }

    public static decimal CalculatePrice(Tariff? tariff, decimal? distanceKm)
    {
        if (tariff == null || distanceKm == null)
            return 0;
        
        decimal price = tariff.BaseFare + (tariff.PricePerKm * distanceKm.Value);
        if (price < tariff.MinFare) price = tariff.MinFare;
        
        return Math.Round(price, 2);
    }
}
