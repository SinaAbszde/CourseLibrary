﻿
namespace CourseLibrary.API.Helpers;
public static class DateTimeOffsetExtensions
{
    public static int GetCurrentAge(this DateTimeOffset dateTimeOffset, DateTimeOffset? dateOfDeath = null)
    {
        var dateToCalculateTo = DateTime.UtcNow;

        if (dateOfDeath is not null)
        {
            dateToCalculateTo = dateOfDeath.Value.UtcDateTime;
        }
        
        var age = dateToCalculateTo.Year - dateTimeOffset.Year;

        if (dateToCalculateTo < dateTimeOffset.AddYears(age))
        {
            age--;
        }

        return age;
    }
}

