using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebEdit.AppData.Entities;

namespace WebEdit.Models;



public class ModelFilter
{
	public string? SearchText { get; set; }

	public string? SelectedMethod { get; set; }
	public List<SelectListItem>? MethodList { get; set; }

	public string? Path { get; set; }
	public string? Method { get; set; }

	public int? StatusCode { get; set; }
	public List<SelectListItem>? StatusCodeList { get; set; }

	public bool? IsHandled { get; set; }

	public DateTime? StartDate { get; set; }
	public DateTime? EndDate { get; set; }




	public int TotalCount { get; set; }
	public int TotalPages { get; set; }
	public int CurrentPage { get; set; } = 1;
	public int PageSize { get; set; } = 20;
	public bool HasPrev => CurrentPage > 1;
	public bool HasNext => CurrentPage < TotalPages;
	public string SortBy { get; set; } = nameof(DbLogEntity.Id);
	public bool SortDesc { get; set; } = true;


}
