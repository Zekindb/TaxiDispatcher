namespace TaxiDispatcher.Models;

public partial class Tariff
{
    public int TariffId { get; set; }
    public string TariffName { get; set; } = null!;
    public decimal BaseFare { get; set; }
    public decimal PricePerKm { get; set; }
    public decimal MinFare { get; set; }
    public int CarClassId { get; set; }
    public virtual CarClass CarClass { get; set; } = null!;
}
