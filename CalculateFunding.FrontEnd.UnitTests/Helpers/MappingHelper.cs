﻿// <copyright file="MappingHelper.cs" company="Department for Education">
// Copyright (c) Department for Education. All rights reserved.
// </copyright>

namespace CalculateFunding.Frontend.Helpers
{
    using AutoMapper;
    using CalculateFunding.Frontend.ViewModels;

    internal class MappingHelper
    {
        internal static IMapper CreateFrontEndMapper()
        {
            MapperConfiguration mappingConfiguration = new MapperConfiguration(c => c.AddProfile<FrontEndMappingProfile>());
            IMapper mapper = mappingConfiguration.CreateMapper();
            return mapper;
        }
    }
}
