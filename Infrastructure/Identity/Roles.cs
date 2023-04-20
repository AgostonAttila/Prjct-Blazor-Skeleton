using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
	internal class Roles
	{
		public const string Admin = nameof(Admin);
		public const string Basic = nameof(Basic);
		public const string Viewer = nameof(Viewer);

		public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[] { Admin, Basic, Viewer });

		public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
	}
}
