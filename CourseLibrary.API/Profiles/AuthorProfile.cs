﻿using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Profiles;
public class AuthorsProfile : Profile
{
    public AuthorsProfile()
    {
        CreateMap<Author, AuthorDto>()
            .ForMember(dest => dest.Name, opt => 
                opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Age, opt => 
                opt.MapFrom(src => src.DateOfBirth.GetCurrentAge(src.DateOfDeath)));

        CreateMap<AuthorForCreationDto, Author>();
        CreateMap<AuthorForCreationWithDateOfDeathDto, Author>();
        CreateMap<Author, AuthorFullDto>();
    }
}

