using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Logging
{
	public static class StaticLogger
	{
		public static void EnsureInitialized()
		{
			if (Log.Logger is not Serilog.Core.Logger)
			{
				Log.Logger = new LoggerConfiguration()
					.Enrich.FromLogContext()
					.WriteTo.Console()
					.CreateLogger();
			}
		}
	}
}
