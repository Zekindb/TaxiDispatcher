using System.Collections.Generic;

namespace TaxiDispatcher.Models;

public partial class OrderStatus
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = null!;
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public override string ToString() => StatusName;
    public override bool Equals(object? obj) => obj is OrderStatus other && StatusId == other.StatusId;
    public override int GetHashCode() => StatusId.GetHashCode();
}
