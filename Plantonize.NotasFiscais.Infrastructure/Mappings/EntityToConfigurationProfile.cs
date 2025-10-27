using AutoMapper;
using Plantonize.NotasFiscais.Domain.Entities;
using Plantonize.NotasFiscais.Domain.Enum;

namespace Plantonize.NotasFiscais.Infrastructure.Mappings
{
    public class EntityToConfigurationProfile : Profile
    {
        public EntityToConfigurationProfile()
        {
            // MedicoFiscal mappings
            CreateMap<MedicoFiscal, Configuration.MedicoFiscalConfiguration>()
                .ReverseMap();

            // TomadorServico mappings
            CreateMap<TomadorServico, Configuration.TomadorServicoConfiguration>()
                .ReverseMap();

            // ItemServico mappings
            CreateMap<ItemServico, Configuration.ItemServicoConfiguration>()
                .ReverseMap();

            // NotaFiscal mappings
            CreateMap<NotaFiscal, Configuration.NFSEConfiguration>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Medico, opt => opt.MapFrom(src => src.Medico))
                .ForMember(dest => dest.Tomador, opt => opt.MapFrom(src => src.Tomador))
                .ForMember(dest => dest.Servicos, opt => opt.MapFrom(src => src.Servicos));

            CreateMap<Configuration.NFSEConfiguration, NotaFiscal>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<StatusNFSEEnum>(src.Status)))
                .ForMember(dest => dest.Medico, opt => opt.MapFrom(src => src.Medico))
                .ForMember(dest => dest.Tomador, opt => opt.MapFrom(src => src.Tomador))
                .ForMember(dest => dest.Servicos, opt => opt.MapFrom(src => src.Servicos));

            // Fatura mappings
            CreateMap<Fatura, Configuration.FaturaConfiguration>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<StatusFaturaEnum>(src.Status)));

            // MunicipioAliquota mappings
            CreateMap<MunicipioAliquota, Configuration.MunicipioAliquotaConfiguration>()
                .ReverseMap();

            // ImpostoResumo mappings
            CreateMap<ImpostoResumo, Configuration.ImpostoResumoConfiguration>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.MedicoId, opt => opt.MapFrom(src => src.MedicoId.ToString()))
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.MedicoId, opt => opt.MapFrom(src => Guid.Parse(src.MedicoId)));
        }
    }
}
