using WebEdit.AppData.Entities;

namespace WebEdit.ViewModels;

public class WebDocViewModel : WebDocEntity
{
	public string ProtectedId { get; set; } = string.Empty;
}
