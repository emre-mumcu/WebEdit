namespace WebEdit.AppData.Entities;

public class WebDocEntity: BaseEntity
{
	public string? Title { get; set; }
	public string? Slug { get; set; }
	public string? Content { get; set; }
	public string? Tags { get; set; }
	public int ViewCount { get; set; }
	public string? Source { get; set; }
}
