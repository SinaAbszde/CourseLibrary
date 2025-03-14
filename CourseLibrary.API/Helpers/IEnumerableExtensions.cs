using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers;

public static class IEnumerableExtensions
{
    public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string? fields)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        
        List<ExpandoObject> expandoObjects = new();
        List<PropertyInfo> propertyInfoList = new();

        if (string.IsNullOrWhiteSpace(fields))
        {
            // all public properties should be in the expando object
            var propertyInfos =
                typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            
            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            var fieldsSplit = fields.Split(',');

            foreach (var field in fieldsSplit)
            {
                var propertyName = field.Trim();

                var propertyInfo = typeof(TSource).GetProperty(propertyName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo is null)
                {
                    throw new Exception($"Property {propertyName} wasn't found in type {typeof(TSource)}");
                }
                
                propertyInfoList.Add(propertyInfo);
            }
        }

        foreach (TSource sourceObject in source)
        {
            ExpandoObject dataShapedObject = new();

            foreach (var propertyInfo in propertyInfoList)
            {
                var propertyValue = propertyInfo.GetValue(sourceObject);

                ((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }
            
            expandoObjects.Add(dataShapedObject);
        }
        
        return expandoObjects;
    }
}