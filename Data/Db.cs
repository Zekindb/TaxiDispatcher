using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace TaxiDispatcher.Data;

public static class Db
{
    public static TaxiContext Create()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "connection.txt");
        string connection = File.ReadAllText(path).Trim();
        var options = new DbContextOptionsBuilder<TaxiContext>()
            .UseSqlServer(connection)
            .Options;
        return new TaxiContext(options);
    }
}
