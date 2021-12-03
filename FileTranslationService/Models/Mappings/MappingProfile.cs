﻿using AutoMapper;
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
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(source => source.File.FileName));

            CreateMap<Models.Database.Task, Models.DTO.Task.Task>();

            CreateMap<Models.DTO.Task.TaskUpdate, Models.Database.Task>();

            #endregion

            #region File mappings

            CreateMap<Models.Database.File, Models.DTO.File.File>();

            CreateMap<Models.DTO.File.NewFile, Models.Database.File>();

            #endregion
        }
    }
}
