using System.Reflection;

namespace CourseLibrary.API.Services;

public class PropertyCheckerService : IPropertyCheckerService
{
    public bool TypeHasProperties<T>(string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return true;
        }
        
        var fieldsSplit = fields.Split(',');

        foreach (var field in fieldsSplit)
        {
            var propertyName = field.Trim();

            var propertyInfo = typeof(T).GetProperty(propertyName,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (propertyInfo is null)
            {
                return false;
            }
        }

        return true;
    }
}