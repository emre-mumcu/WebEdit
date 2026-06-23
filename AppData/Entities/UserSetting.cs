using System.ComponentModel.DataAnnotations;

namespace WebEdit.AppData.Entities;

public class UserSetting
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(450)]
    public string UserId { get; set; } = default!;

    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = default!;

    [MaxLength(500)]
    public string? Value { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
