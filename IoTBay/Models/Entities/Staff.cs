using System.ComponentModel.DataAnnotations.Schema;

namespace IoTBay.Models.Entities;

public enum Permission
{
    Clerk, Manager, Admin
}

public partial class Staff
{
    public int StaffId { get; set; }

    public int UserId { get; set; }
    
    [Column("permission", TypeName = "permission_enum")] // PostgreSQL enum type name
    public Permission Permission { get; set; }

    public virtual User User { get; set; } = null!;
}
