using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.BackgroundJobs
{
	public class HangfireStorageSettings
	{
		public string? StorageProvider { get; set; }
		public string? ConnectionString { get; set; }
	}
}
