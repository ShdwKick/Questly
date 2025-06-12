namespace DataModels.DTOs;

public class BlockUserDTO
{
    public Guid UserId { get; set; }
    public bool BlockStatus { get; set; }
    public string? Reason { get; set; }
}