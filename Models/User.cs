using System;
using System.Collections.Generic;

namespace TaxiDispatcher.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
}
