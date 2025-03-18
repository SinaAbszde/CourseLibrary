using System.Text.Json;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;

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

        var paginationMetaData = new
        {
            totalCount = authorsFromRepo.TotalCount,
            pageSize = authorsFromRepo.PageSize,
            currentPage = authorsFromRepo.CurrentPage,
            totalPages = authorsFromRepo.TotalPages
        };

        var links = CreateLinksForAuthors(
            authorsResourceParameters,
            authorsFromRepo.HasNextPage,
            authorsFromRepo.HasPreviousPage);

        var shapedAuthors = _mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo)
            .ShapeData(authorsResourceParameters.Fields);

        var shapedAuthorsWithLinks = shapedAuthors.Select(author =>
        {
            IDictionary<string, object?> authorAsDictionary = author;
            var authorLinks = CreateLinksForAuthor((Guid)authorAsDictionary["Id"]!, null);
            authorAsDictionary.Add("links", authorLinks);
            return authorAsDictionary;
        });

        var linkedCollectionResource = new
        {
            value = shapedAuthorsWithLinks,
            links
        };

        Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(paginationMetaData));
        
        // return them
        return Ok(linkedCollectionResource);
    }

    private IEnumerable<LinkDto> CreateLinksForAuthors(AuthorsResourceParameters authorsResourceParameters,
        bool hasNextPage, bool hasPreviousPage)
    {
        List<LinkDto> links = new()
        {
            new LinkDto(
                CreateAuthorsResourceUri(
                    authorsResourceParameters,
                    ResourceUriType.CurrentPage),
                "self",
                "GET")
        };
        if (hasNextPage)
            links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.NextPage),
                "nextPage", "GET"));
        if (hasPreviousPage)
            links.Add(new LinkDto(CreateAuthorsResourceUri(authorsResourceParameters, ResourceUriType.PreviousPage),
                "previousPage", "GET"));
        
        return links;
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
            ResourceUriType.CurrentPage => Url.Link(nameof(GetAuthors),
                new
                {
                    fields = authorsResourceParameters.Fields,
                    orderBy = authorsResourceParameters.OrderBy,
                    pageNumber = authorsResourceParameters.PageNumber,
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

    [Produces(
        "application/json",
        "application/vnd.marvin.hateoas+json",
        "application/vnd.marvin.author.full+json",
        "application/vnd.marvin.author.full.hateoas+json",
        "application/vnd.marvin.author.friendly+json",
        "application/vnd.marvin.author.friendly.hateoas+json")]
    [HttpGet("{authorId}", Name = "GetAuthor")]
    public async Task<IActionResult> GetAuthor(Guid authorId, string? fields, [FromHeader(Name = "Accept")] string? mediaType)
    {
        if (!MediaTypeHeaderValue.TryParse(mediaType, out var parsedMediaType))
        {
            return BadRequest(
                _problemDetailsFactory.CreateProblemDetails(HttpContext, StatusCodes.Status400BadRequest,
                    detail: $"Accept header media type value is not a valid media type: {mediaType}"));
        }
        
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

        var includeLinks =
            parsedMediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        IEnumerable<LinkDto> links = new List<LinkDto>();
        if (includeLinks)
        {
            links = CreateLinksForAuthor(authorId, fields);
        }

        var primaryMediaType = includeLinks
            ? parsedMediaType.SubTypeWithoutSuffix.Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
            : parsedMediaType.SubTypeWithoutSuffix;

        // full author
        if (primaryMediaType == "vnd.marvin.author.full")
        {
            IDictionary<string, object?> fullResourceToReturn = _mapper.Map<AuthorFullDto>(authorFromRepo).ShapeData(fields);
            if (includeLinks)
            {
                fullResourceToReturn.Add("links", links);
            }
            
            return Ok(fullResourceToReturn);
        }
        
        // friendly author
        IDictionary<string, object?> friendlyResourceToReturn =
            _mapper.Map<AuthorDto>(authorFromRepo).ShapeData(fields);

        if (includeLinks)
        {
            friendlyResourceToReturn.Add("links", links);
        }

        return Ok(friendlyResourceToReturn);
    }

    private IEnumerable<LinkDto> CreateLinksForAuthor(Guid authorId, string? fields)
    {
        List<LinkDto> links = new()
        {
            string.IsNullOrWhiteSpace(fields)
                ? new LinkDto(Url.Link("GetAuthor", new { authorId }), "self", "GET")
                : new LinkDto(Url.Link("GetAuthor", new { authorId, fields }), "self", "GET"),
            new LinkDto(Url.Link("CreateCourseForAuthor", new { authorId }), "create_course_for_author", "POST"),
            new LinkDto(Url.Link("GetCoursesForAuthor", new { authorId }), "courses", "GET")
        };

        return links;
    }

    [HttpPost(Name = "CreateAuthor")]
    public async Task<ActionResult<AuthorDto>> CreateAuthor(AuthorForCreationDto author)
    {
        var authorEntity = _mapper.Map<Author>(author);

        _courseLibraryRepository.AddAuthor(authorEntity);
        await _courseLibraryRepository.SaveAsync();

        var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);

        var links = CreateLinksForAuthor(authorToReturn.Id, null);

        IDictionary<string, object?> linkedResourceToReturn = authorToReturn.ShapeData(null);
        
        linkedResourceToReturn.Add("links", links);
        
        return CreatedAtRoute("GetAuthor",
            new { authorId = linkedResourceToReturn["Id"] },
            linkedResourceToReturn);
    }

    [HttpOptions]
    public IActionResult GetAuthorsOptions()
    {
        Response.Headers.Add("Allow", "GET,HEAD,POST,OPTIONS");
        return Ok();
    }
}
