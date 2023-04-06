using Domain;
using Domain.DTOs;

namespace Application.Core
{
	public class MappingProfiles : AutoMapper.Profile
	{
		public MappingProfiles()
		{
			CreateMap<RegisterDTO, AppUser>();
		}
	}
}
