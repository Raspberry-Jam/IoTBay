using IoTBay.Models.Entities;

namespace IoTBay.Models.DTOs;

public class UserSessionDto
{
    public int UserId { get; set; }
    public required string GivenName { get; set; }
    public Role Role { get; set; }
}