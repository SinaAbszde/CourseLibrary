using System.Linq.Dynamic.Core;
using CourseLibrary.API.Services;

namespace CourseLibrary.API.Helpers;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
        Dictionary<string, PropertyMappingValue> mappingDictionary)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (mappingDictionary is null)
        {
            throw new ArgumentNullException(nameof(mappingDictionary));
        }

        if (string.IsNullOrWhiteSpace(orderBy))
        {
            return source;
        }

        var orderByString = string.Empty;
        
        // The orderBy string is separated by "," so we split it
        var orderBySplit = orderBy.Split(",");

        foreach (var orderByClause in orderBySplit)
        {
            var trimmedOrderByClause = orderByClause.Trim();
            
            var descendingOrder = trimmedOrderByClause.EndsWith(" desc");
            
            var indexofFirstSpace = trimmedOrderByClause.IndexOf(' ');
            var propertyName = indexofFirstSpace == -1
                ? trimmedOrderByClause
                : trimmedOrderByClause[..indexofFirstSpace];

            if (!mappingDictionary.TryGetValue(propertyName, out var propertyMappingValue))
            {
                throw new ArgumentException($"Key mapping for {propertyName} was not found in Mapping dictionary.");
            }
            
            if (propertyMappingValue is null)
            {
                throw new ArgumentException($"Key mapping for {propertyName} was not found in Mapping dictionary.");
            }

            if (propertyMappingValue.Revert)
            {
                descendingOrder = !descendingOrder;
            }

            orderByString = propertyMappingValue.DestinationProperties.Aggregate(orderByString,
                (current, destinationProperty) => current + (string.IsNullOrWhiteSpace(current) ? string.Empty : ", ") +
                                                  destinationProperty +
                                                  (descendingOrder ? " descending" : " ascending"));
        }
        
        return source.OrderBy(orderByString);
    }
}