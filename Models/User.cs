namespace TaxiDispatcher.Models;

public class User
{
    public int UserId { get; set; }
    public string Login { get; set; } = "";
    public string FullName { get; set; } = "";
    public int RoleId { get; set; }
    public virtual Role? Role { get; set; }
}
