using System;

namespace WebEdit.AppData.Entities;

public class ExceptionLogEntity: BaseEntity
{
	public string? UserId { get; set; }
	public string? Path { get; set; }
	public string? Method { get; set; }
	public string? QueryString { get; set; }
	public int? StatusCode { get; set; }
	public string? Message { get; set; }
	public string? StackTrace { get; set; }
	public string? InnerException { get; set; }
	public bool IsHandled { get; set; }
	public string? ExceptionType { get; set; }
}
