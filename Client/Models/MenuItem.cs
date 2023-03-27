using System;

namespace Client.Models
{
	public class MenuItem
	{
		public string? Link { get; set; }
		public string? NormalClass { get; set; }
		public string? NormalTitle { get; set; }
		public string? MiniClass { get; set; }
		public string? MiniTitle { get; set; }
	}

	public class MainMenuItem
	{
        public string Name { get; set; }
		public List<MenuItem> MenuItemList { get; set; } = new List<MenuItem>();
	}
}


