using System;
using WebEdit.AppData.Entities;
using WebEdit.Models;

namespace WebEdit.ViewModels;

public class DbLogViewModel
{
	public List<DbLogEntity> Items { get; set; } = new();

	public ModelFilter ModelFilter { get; set; } = new();

}
