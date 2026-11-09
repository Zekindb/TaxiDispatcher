namespace TaxiDispatcher.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } = null!;
    public virtual Order Order { get; set; } = null!;
}
