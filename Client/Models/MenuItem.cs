using Client.Models;
using System;

namespace Client.Models
{
	public class MenuItem
	{
		public string Id { get; set; }
		public string? Link { get; set; }
		public string? Title { get; set; }
		public List<MenuItem> MenuItemList { get; set; } = new List<MenuItem>();
	}

	public class SideBarMenuItem
	{
		public int SequenceId { get; set; }
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? Icon { get; set; }
		public string? Type { get; set; }
		public List<MenuItem> MainMenuItemList { get; set; } = new List<MenuItem>();

	}	
}


