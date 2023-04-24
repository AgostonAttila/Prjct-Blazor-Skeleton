using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
	public static class OwnAction
	{
		public const string View = nameof(View);
		public const string Search = nameof(Search);
		public const string Create = nameof(Create);
		public const string Update = nameof(Update);
		public const string Delete = nameof(Delete);
		public const string Export = nameof(Export);
		public const string Generate = nameof(Generate);
		public const string Clean = nameof(Clean);
		public const string UpgradeSubscription = nameof(UpgradeSubscription);
	}

	public static class OwnResource
	{		
		public const string Dashboard = nameof(Dashboard);
		public const string Hangfire = nameof(Hangfire);
		public const string Users = nameof(Users);
		public const string UserRoles = nameof(UserRoles);
		public const string Roles = nameof(Roles);
		public const string RoleClaims = nameof(RoleClaims);
		public const string Products = nameof(Products);
		//public const string Tenants = nameof(Tenants);
		//public const string Brands = nameof(Brands);
	}

	public static class OwnPermissions
	{
		private static readonly OwnPermission[] _all = new OwnPermission[]
		{
		new("View Dashboard", OwnAction.View, OwnResource.Dashboard),
		new("View Hangfire", OwnAction.View, OwnResource.Hangfire),
		new("View Users", OwnAction.View, OwnResource.Users),
		new("Search Users", OwnAction.Search, OwnResource.Users),
		new("Create Users", OwnAction.Create, OwnResource.Users),
		new("Update Users", OwnAction.Update, OwnResource.Users),
		new("Delete Users", OwnAction.Delete, OwnResource.Users),
		new("Export Users", OwnAction.Export, OwnResource.Users),
		new("View UserRoles", OwnAction.View, OwnResource.UserRoles),
		new("Update UserRoles", OwnAction.Update, OwnResource.UserRoles),
		new("View Roles", OwnAction.View, OwnResource.Roles),
		new("Create Roles", OwnAction.Create, OwnResource.Roles),
		new("Update Roles", OwnAction.Update, OwnResource.Roles),
		new("Delete Roles", OwnAction.Delete, OwnResource.Roles),
		new("View RoleClaims", OwnAction.View, OwnResource.RoleClaims),
		new("Update RoleClaims", OwnAction.Update, OwnResource.RoleClaims),
		new("View Products", OwnAction.View, OwnResource.Products, IsBasic: true),
		new("Search Products", OwnAction.Search, OwnResource.Products, IsBasic: true),
		new("Create Products", OwnAction.Create, OwnResource.Products),
		new("Update Products", OwnAction.Update, OwnResource.Products),
		new("Delete Products", OwnAction.Delete, OwnResource.Products),
		new("Export Products", OwnAction.Export, OwnResource.Products),
		//new("View Brands", OwnAction.View, OwnResource.Brands, IsBasic: true),
		//new("Search Brands", OwnAction.Search, OwnResource.Brands, IsBasic: true),
		//new("Create Brands", OwnAction.Create, OwnResource.Brands),
		//new("Update Brands", OwnAction.Update, OwnResource.Brands),
		//new("Delete Brands", OwnAction.Delete, OwnResource.Brands),
		//new("Generate Brands", OwnAction.Generate, OwnResource.Brands),
		//new("Clean Brands", OwnAction.Clean, OwnResource.Brands),
		//new("View Tenants", OwnAction.View, OwnResource.Tenants, IsRoot: true),
		//new("Create Tenants", OwnAction.Create, OwnResource.Tenants, IsRoot: true),
		//new("Update Tenants", OwnAction.Update, OwnResource.Tenants, IsRoot: true),
		//new("Upgrade Tenant Subscription", OwnAction.UpgradeSubscription, OwnResource.Tenants, IsRoot: true)
		};

		public static IReadOnlyList<OwnPermission> All { get; } = new ReadOnlyCollection<OwnPermission>(_all);
		public static IReadOnlyList<OwnPermission> Root { get; } = new ReadOnlyCollection<OwnPermission>(_all.Where(p => p.IsRoot).ToArray());
		public static IReadOnlyList<OwnPermission> Admin { get; } = new ReadOnlyCollection<OwnPermission>(_all.Where(p => !p.IsRoot).ToArray());
		public static IReadOnlyList<OwnPermission> Basic { get; } = new ReadOnlyCollection<OwnPermission>(_all.Where(p => p.IsBasic).ToArray());
	
	}

	public record OwnPermission(string Description, string Action, string Resource, bool IsBasic = false, bool IsRoot = false)
	{
		public string Name => NameFor(Action, Resource);
		public static string NameFor(string action, string resource) => $"Permissions.{resource}.{action}";
	}
}
