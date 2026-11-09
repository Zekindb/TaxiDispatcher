using System.Collections.Generic;

namespace TaxiDispatcher.Models;

public partial class CarClass
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = null!;
    public virtual ICollection<Tariff> Tariffs { get; set; } = new List<Tariff>();
}
