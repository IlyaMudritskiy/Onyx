using AutoMapper;
using Onyx.Models.Domain.ProcessData;
namespace Onyx.Mappers
{
    public class ProcessDataMapper : Profile
    {
        public ProcessDataMapper()
        {
            CreateMap<ProcessDataModel, NewProcessDataModel>();
            CreateMap<NewProcessDataModel, ProcessDataModel>();
            CreateMap<ProcessDataModel, ShortProcessDataModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.DUT, opt => opt.MapFrom(src => src.DUT));
        }
    }
}
