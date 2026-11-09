using System;

namespace TaxiDispatcher.Models;

public partial class Order
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string ClientName { get; set; } = null!;
    public string ClientPhone { get; set; } = null!;
    public string PickupAddress { get; set; } = null!;
    public string DestinationAddress { get; set; } = null!;
    public decimal? DistanceKm { get; set; }
    public int StatusId { get; set; }
    public int? TariffId { get; set; }
    
    public virtual OrderStatus Status { get; set; } = null!;
    public virtual Tariff? Tariff { get; set; }
    public virtual Invoice? Invoice { get; set; }
}
