namespace Core.Application.Entities;

public class BaseEntity
{
    public bool Active { get; set; }
    
    public DateTime? CreationDate { get; set; }

    public DateTime? LastUpdatedDate { get; set; }
}