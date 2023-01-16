using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands;

public class CommandHandler : ICommandHandler
{
    private readonly IEventSourcingHandler<PostAggregate> eventSourcingHandler;

    public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        this.eventSourcingHandler = eventSourcingHandler;
    }

    public async Task HandleAync(NewPostCommand command)
    {
        var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAync(EditMessageCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.EditMessage(command.Message);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAync(LikePostCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.LikePost();

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAync(AddCommentCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.AddComment(command.Comment, command.Username);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAync(EditCommentCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.EditComment(command.Comment, command.Username, command.CommentId);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAync(RemoveCommentCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId, command.Username);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAync(DeletePostCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.DeletePost(command.Username);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }
}