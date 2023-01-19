using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RestoreReadDbController : ControllerBase
    {
        private readonly ILogger<RestoreReadDbController> logger;
        private readonly ICommandDispatcher commandDispatacher;

        public RestoreReadDbController(ICommandDispatcher commandDispatacher, ILogger<RestoreReadDbController> logger)
        {
            this.commandDispatacher = commandDispatacher;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> RestoreReadDbAsync()
        {
            try
            {
                await this.commandDispatacher.SendAsync(new RestoreReadDbCommand());
                return StatusCode(StatusCodes.Status201Created, new BaseResponse
                {
                    Message = "Read database restore request completed successfully!"
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
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to restore read database!";
                this.logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = SAFE_ERROR_MESSAGE
                });
            }
        }
    }
}