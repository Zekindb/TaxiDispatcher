using Microsoft.EntityFrameworkCore;
using TaxiDispatcher.Models;

namespace TaxiDispatcher.Data;

public class TaxiContext : DbContext
{
    public TaxiContext(DbContextOptions<TaxiContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
}
