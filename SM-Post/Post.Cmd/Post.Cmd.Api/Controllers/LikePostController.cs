using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class LikePostController : ControllerBase
{
    private readonly ILogger<LikePostController> logger;
    private readonly ICommandDispatcher commandDispatacher;

    public LikePostController(ICommandDispatcher commandDispatacher, ILogger<LikePostController> logger)
    {
        this.commandDispatacher = commandDispatacher;
        this.logger = logger;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> LikePostAsync(Guid id)
    {
        try
        {
            await this.commandDispatacher.SendAsync(new LikePostCommand { Id = id });

            return Ok(new BaseResponse
            {
                Message = "Like post request completed successfully!!"
            });
        }
        catch (InvalidOperationException ex)
        {
            this.logger.Log(LogLevel.Warning, ex, "Client made a bad request!");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message,
            });
        }
        catch (AggregateNotFoundException ex)
        {
            this.logger.Log(LogLevel.Warning, ex, "Could not retrieve aggregate, client passed an incorrect post ID targeting the aggregate!");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message,
            });
        }
        catch (Exception ex)
        {
            const string SAFE_ERROR_MESSAGE = "Error while processing request to like a post!";
            this.logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
            {
                Id = id,
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}