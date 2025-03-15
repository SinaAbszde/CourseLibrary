using System.Text.Json;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers;

[ApiController] 
[Route("api/authors")]
public class AuthorsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IPropertyCheckerService _propertyCheckerService;
    private readonly IPropertyMappingService _propertyMappingService;
    private readonly ICourseLibraryRepository _courseLibraryRepository;

    public AuthorsController(IMapper mapper, ProblemDetailsFactory problemDetailsFactory,
        IPropertyCheckerService propertyCheckerService, IPropertyMappingService propertyMappingService,
        ICourseLibraryRepository courseLibraryRepository)
    {
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
        _problemDetailsFactory =
            problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
        _propertyCheckerService =
            propertyCheckerService ?? throw new ArgumentNullException(nameof(propertyCheckerService));
        _propertyMappingService =
            propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
        _courseLibraryRepository = courseLibraryRepository ??
            throw new ArgumentNullException(nameof(courseLibraryRepository));
    }

    [HttpGet(Name = "GetAuthors")] 
    [HttpHead]
    public async Task<IActionResult> GetAuthors(
        [FromQuery] AuthorsResourceParameters authorsResourceParameters)
    {
        if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>(authorsResourceParameters.OrderBy))
        {
            return BadRequest();
        }

        if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(authorsResourceParameters.Fields))
        {
            return BadRequest(_problemDetailsFactory.CreateProblemDetails(
                HttpContext,
                StatusCodes.Status400BadRequest,
                detail:
                $"Not all requested data shaping fields exist on the resource: {authorsResourceParameters.Fields}"));
        }
        
        // get authors from repo
        var authorsFromRepo = await _courseLibraryRepository
            .GetAuthorsAsync(authorsResourceParameters);

        var previousPageLink = authorsFromRepo.HasPreviousPage
            ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage)
            : null;

        var nextPageLink = authorsFromRepo.HasNextPage
            ? CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage)
            : null;

        var paginationMetaData = new
        {
            totalCount = authorsFromRepo.TotalCount,
            pageSize = authorsFromRepo.PageSize,
            currentPage = authorsFromRepo.CurrentPage,
            totalPages = authorsFromRepo.TotalPages,
            previousPageLink,
            nextPageLink
        };

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetaData));
        
        // return them
        return Ok(_mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo).ShapeData(authorsResourceParameters.Fields));
    }

    private string? CreateAuthorsResourceUri(AuthorsResourceParameters authorsResourceParameters, ResourceUriType type)
    {
        return type switch
        {
            ResourceUriType.PreviousPage => Url.Link(nameof(GetAuthors),
                new
                {
                    fields = authorsResourceParameters.Fields,
                    orderBy = authorsResourceParameters.OrderBy,
                    pageNumber = authorsResourceParameters.PageNumber - 1,
                    pageSize = authorsResourceParameters.PageSize,
                    maincategory = authorsResourceParameters.MainCategory,
                    searchQuery = authorsResourceParameters.SearchQuery
                }),
            ResourceUriType.NextPage => Url.Link(nameof(GetAuthors),
                new
                {
                    fields = authorsResourceParameters.Fields,
                    orderBy = authorsResourceParameters.OrderBy,
                    pageNumber = authorsResourceParameters.PageNumber + 1,
                    pageSize = authorsResourceParameters.PageSize,
                    maincategory = authorsResourceParameters.MainCategory,
                    searchQuery = authorsResourceParameters.SearchQuery
                }),
            _ => Url.Link(nameof(GetAuthors),
                new
                {
                    fields = authorsResourceParameters.Fields,
                    orderBy = authorsResourceParameters.OrderBy,
                    pageNumber = authorsResourceParameters.PageNumber,
                    pageSize = authorsResourceParameters.PageSize,
                    maincategory = authorsResourceParameters.MainCategory,
                    searchQuery = authorsResourceParameters.SearchQuery
                })
        };
    }

    [HttpGet("{authorId}", Name = "GetAuthor")]
    public async Task<IActionResult> GetAuthor(Guid authorId, string? fields)
    {
        if (!_propertyCheckerService.TypeHasProperties<AuthorDto>(fields))
        {
            return BadRequest(_problemDetailsFactory.CreateProblemDetails(
                HttpContext,
                StatusCodes.Status400BadRequest,
                detail:
                $"Not all requested data shaping fields exist on the resource: {fields}"));
        }
        
        // get author from repo
        var authorFromRepo = await _courseLibraryRepository
            .GetAuthorAsync(authorId);

        if (authorFromRepo == null)
        {
            return NotFound();
        }

        // return author
        return Ok(_mapper.Map<AuthorDto>(authorFromRepo).ShapeData(fields));
    }

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorForCreationDto author)
    {
        var authorEntity = _mapper.Map<Entities.Author>(author);

        _courseLibraryRepository.AddAuthor(authorEntity);
        await _courseLibraryRepository.SaveAsync();

        var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

        return CreatedAtRoute("GetAuthor",
            new { authorId = authorToReturn.Id },
            authorToReturn);
    }

    [HttpOptions]
    public IActionResult GetAuthorsOptions()
    {
        Response.Headers.Add("Allow", "GET,HEAD,POST,OPTIONS");
        return Ok();
    }
}
