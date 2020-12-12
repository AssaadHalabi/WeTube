﻿using AutoMapper;
using YouTubeClone.Models;
using YouTubeClone.Models.Dtos;

namespace YouTubeClone.Mappings.Profiles
{
    public class VideoProfile : Profile
    {
        public VideoProfile()
        {
            CreateMap<Video, VideoSummaryDto>()
                .ForMember(dest => dest.ViewsCount, opt => opt.MapFrom(src=> src.UserVideoViews.Count));
            CreateMap<Video, VideoDto>()
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.UserVideoComments))
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src.UserVideoReactions))
                .ForMember(dest => dest.Views, opt => opt.MapFrom(src => src.UserVideoViews))
                .ForMember(dest => dest.ViewsCount, opt => opt.MapFrom(src=> src.UserVideoViews.Count));
        }
    }
}
