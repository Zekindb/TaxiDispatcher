using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace TaxiDispatcher.Data;

public static class Db
{
    public static TaxiContext Create()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "connection.txt");
        string connection = File.Exists(path) 
            ? File.ReadAllText(path).Trim() 
            : "Server=localhost;Database=taxi_service;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True";
            
        var options = new DbContextOptionsBuilder<TaxiContext>()
            .UseSqlServer(connection)
            .Options;
        return new TaxiContext(options);
    }
}
