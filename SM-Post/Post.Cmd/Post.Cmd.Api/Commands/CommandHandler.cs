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

    public async Task HandleAsync(NewPostCommand command)
    {
        var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditMessageCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.EditMessage(command.Message);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(LikePostCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.LikePost();

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(AddCommentCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.AddComment(command.Comment, command.Username);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.EditComment(command.Comment, command.Username, command.CommentId);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.RemoveComment(command.CommentId, command.Username);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
        var aggregate = await this.eventSourcingHandler.GetIdAsync(command.Id);
        aggregate.DeletePost(command.Username);

        await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RestoreReadDbCommand command)
    {
        await this.eventSourcingHandler.RepublishEventsAsync();
    }
}