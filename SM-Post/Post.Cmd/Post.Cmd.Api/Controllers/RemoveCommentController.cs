using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class RemoveCommentController : ControllerBase
{
    private readonly ILogger<RemoveCommentController> logger;
    private readonly ICommandDispatcher commandDispatacher;

    public RemoveCommentController(ICommandDispatcher commandDispatacher, ILogger<RemoveCommentController> logger)
    {
        this.commandDispatacher = commandDispatacher;
        this.logger = logger;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveCommentAsync(Guid id, RemoveCommentCommand command)
    {
        try
        {
            command.Id = id;
            await this.commandDispatacher.SendAsync(command);

            return Ok(new BaseResponse
            {
                Message = "Remove comment request completed successfully!!"
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
            const string SAFE_ERROR_MESSAGE = "Error while processing request to remove a comment from a post!";
            this.logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse
            {
                Id = id,
                Message = SAFE_ERROR_MESSAGE
            });
        }
    }
}