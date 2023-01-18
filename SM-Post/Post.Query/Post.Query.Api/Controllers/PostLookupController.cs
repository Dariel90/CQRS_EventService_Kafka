using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class PostLookupController : ControllerBase
{
    private readonly IQueryDispatcher<PostEntity> queryDispatcher;
    private readonly ILogger<PostLookupController> logger;

    public PostLookupController(IQueryDispatcher<PostEntity> queryDispatcher, ILogger<PostLookupController> logger)
    {
        this.queryDispatcher = queryDispatcher;
        this.logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPostsAsync()
    {
        try
        {
            var posts = await this.queryDispatcher.SendAsync(new FindAllPostsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to retrieve all posts!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("byId/{postId}")]
    public async Task<ActionResult> GetPostIdAsync(Guid id)
    {
        try
        {
            var posts = await this.queryDispatcher.SendAsync(new FindPostByIdQuery { Id = id });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to find the post by ID!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetPostsByAuthorAsync(string author)
    {
        try
        {
            var posts = await this.queryDispatcher.SendAsync(new FindPostsByAuthorQuery { Author = author });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to find the posts by author!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("withComments")]
    public async Task<ActionResult> GetPostsWithCommentsAsync()
    {
        try
        {
            var posts = await this.queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to find the posts with comments!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
    {
        try
        {
            var posts = await this.queryDispatcher.SendAsync(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes });
            return NormalResponse(posts);
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to find the posts with likes!";
            return ErrorResponse(ex, SAFE_ERROR_MESSAGE);
        }
    }

    private ActionResult ErrorResponse(Exception ex, string safeErrorMessage)
    {
        this.logger.LogError(ex, safeErrorMessage);

        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
        {
            Message = safeErrorMessage,
        });
    }

    private ActionResult NormalResponse(List<PostEntity> posts)
    {
        if (posts == null || !posts.Any())
            return NoContent();
        var count = posts.Count();
        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned {count} post {(count > 1 ? "s" : string.Empty)}!"
        });
    }
}