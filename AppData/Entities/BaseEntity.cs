namespace WebEdit.AppData.Entities;

public class BaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime? UpdatedAt { get; set; } 
	public string? CreatedBy { get; set; } 
	public string? UpdatedBy { get; set; } 
}
