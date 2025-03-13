﻿using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Services;

public class PropertyMappingService : IPropertyMappingService
{
    private readonly Dictionary<string, PropertyMappingValue> _authorsPropertyMapping =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "Id", new PropertyMappingValue(new[] { "Id" }) },
            { "MainCategory", new PropertyMappingValue(new[] { "MainCategory" }) },
            { "Age", new PropertyMappingValue(new[] { "DateOfBirth" }, true) },
            { "Name", new PropertyMappingValue(new[] { "FirstName", "LastName" }) }
        };

    private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

    public PropertyMappingService()
    {
        _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorsPropertyMapping));
    }
    public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
    {
        // Get matching mapping
        var matchingMapping = _propertyMappings.OfType<PropertyMapping<AuthorDto, Author>>().ToList();

        if (matchingMapping.Count == 1)
        {
            return matchingMapping.First().MappingDictionary;
        }

        throw new Exception(
            $"Cannot find exact property mapping instance for <{typeof(TSource)}, {typeof(TDestination)}>");
    }
}