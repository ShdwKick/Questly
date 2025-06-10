namespace DataModels;

public class UserForCreate
{
    public string Username { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
    public Guid CityId { get; set; }
}