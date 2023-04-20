using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
	public interface ICurrentUserInitializer
	{
		void SetCurrentUser(ClaimsPrincipal user);

		void SetCurrentUserId(string userId);
	}
}
