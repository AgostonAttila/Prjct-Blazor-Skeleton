using Infrastructure._Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using OrchardCore.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Localization
{
	internal class OwnFileLocationProvider : ILocalizationFileLocationProvider
	{
		private readonly IFileProvider _fileProvider;
		private readonly string _resourcesContainer;

		public OwnFileLocationProvider(IHostEnvironment hostingEnvironment, IOptions<LocalizationOptions> localizationOptions)
		{
			_fileProvider = hostingEnvironment.ContentRootFileProvider;
			_resourcesContainer = localizationOptions.Value.ResourcesPath;
		}
		public IEnumerable<IFileInfo> GetLocations(string cultureName)
		{
			// Loads all *.po files from the culture folder under the Resource Path.
			// for example, src\Host\Localization\en-US\FSH.Exceptions.po
			foreach (var file in _fileProvider.GetDirectoryContents(PathExtensions.Combine(_resourcesContainer, cultureName)))
			{
				yield return file;
			}
		}
	}
}
