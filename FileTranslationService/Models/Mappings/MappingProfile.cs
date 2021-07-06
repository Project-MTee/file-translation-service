using AutoMapper;
using Tilde.MT.FileTranslationService.Models.DTO.File;
using Tilde.MT.FileTranslationService.Models.DTO.Task;

namespace Tilde.MT.FileTranslationService.Models.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            #region Metadata mappings

            CreateMap<Models.DTO.Task.NewTask, Models.Database.Task>()
                .ForMember(dest => dest.TranslationStatus, opt => opt.Ignore())
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(source => source.File.FileName));

            CreateMap<Models.Database.Task, Models.DTO.Task.Task>()
                .ForMember(dest => dest.TranslationStatus, opt => opt.MapFrom(source => source.TranslationStatus.Status));

            CreateMap<Models.DTO.Task.TaskUpdate, Models.Database.Task>()
                .ForMember(x => x.TranslationStatus, opt => opt.Ignore());

            #endregion

            #region File mappings

            CreateMap<Models.Database.File, Models.DTO.File.File>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(source => source.Category.Category));

            CreateMap<Models.DTO.File.NewFile, Models.Database.File>()
                .ForMember(x => x.Category, opt => opt.Ignore());

            #endregion
        }
    }
}
