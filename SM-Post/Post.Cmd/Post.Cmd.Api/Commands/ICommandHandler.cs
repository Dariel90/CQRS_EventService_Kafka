namespace Post.Cmd.Api.Commands;

public interface ICommandHandler
{
    Task HandleAync(NewPostCommand command);

    Task HandleAync(EditMessageCommand command);

    Task HandleAync(LikePostCommand command);

    Task HandleAync(AddCommentCommand command);

    Task HandleAync(EditCommentCommand command);

    Task HandleAync(RemoveCommentCommand command);

    Task HandleAync(DeletePostCommand command);
}