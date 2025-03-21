﻿namespace CourseLibrary.API.ResourceParameters;

public class AuthorsResourceParameters
{
    private const int MaxPageSize = 20;
    public string? MainCategory { get; set; }
    public string? SearchQuery { get; set; }
    public int PageNumber { get; set; } = 1;
    private int _pageSize { get; set; } = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string OrderBy { get; set; } = "Name";
    public string? Fields { get; set; }
}