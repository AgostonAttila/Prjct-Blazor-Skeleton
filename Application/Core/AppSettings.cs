using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core
{
	public class AppSettings
	{		
		public int RefreshTokenTTL { get; set; }
		public string? EmailFrom { get; set; }
		public string? SmtpHost { get; set; }
		public int SmtpPort { get; set; }
		public string? SmtpUser { get; set; }
		public string? SmtpPass { get; set; }
	}
}
