using System.ComponentModel.DataAnnotations.Schema;

namespace TaxiDispatcher.Models;

public partial class User
{
    public int UserId { get; set; }
    public string Login { get; set; } = null!;
    
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }

    public string? Phone { get; set; }
    public int RoleId { get; set; }
    public string? PasswordHash { get; set; }
    public virtual Role Role { get; set; } = null!;

    [NotMapped]
    public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
}
