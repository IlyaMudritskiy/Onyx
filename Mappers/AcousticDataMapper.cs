using AutoMapper;
using Onyx.Models.Domain.AcousticData;

namespace Onyx.Mappers
{
    public class AcousticDataMapper: Profile
    {
        public AcousticDataMapper()
        {
            CreateMap<AcousticDataModel, NewAcousticDataModel>();
            CreateMap<NewAcousticDataModel, AcousticDataModel>();
            CreateMap<AcousticDataModel, ShortAcousticDataModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DUT, opt => opt.MapFrom(src => src.DUT));
        }
    }
}
