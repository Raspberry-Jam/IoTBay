using IoTBay.Models.Entities;

namespace IoTBay.Models.DTOs;

/// <summary>
/// A simple minimum-required set of data for tracking the user's state across pages on the browser session
/// </summary>
public class UserSessionDto
{
    public int UserId { get; set; }
    public required string GivenName { get; set; }
    public Role Role { get; set; }
}