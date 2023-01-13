using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool active;
    private string author;
    private readonly Dictionary<Guid, Tuple<string, string>> comments = new();

    public bool Active { get => this.active; set => this.active = value; }

    public PostAggregate()
    {
    }

    public PostAggregate(Guid id, string author, string message)
    {
        this.RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            DatePosted = DateTime.Now,
            Message = message,
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        this.Id = @event.Id;
        this.active = true;
        this.author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!this.active)
        {
            throw new InvalidOperationException("You cannot edit message of an inactive post!");
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty. Please provide a valid {nameof(message)}");
        }

        this.RaiseEvent(new MessageUpdatedEvent
        {
            Id = this.Id,
            Message = message,
        });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        this.Id = @event.Id;
    }

    public void LikePost()
    {
        if (!this.active)
        {
            throw new InvalidOperationException("You cannot like an inactive post!");
        }

        this.RaiseEvent(new PostLikedEvent
        {
            Id = this.Id,
        });
    }

    public void Apply(PostLikedEvent @event)
    {
        this.Id = @event.Id;
    }

    public void AddComment(string comment, string username)
    {
        if (!this.active)
        {
            throw new InvalidOperationException("You cannot add a comment of an inactive post!");
        }
        if (string.IsNullOrWhiteSpace(comment))
        {
            throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty. Please provide a valid {nameof(comment)}");
        }

        this.RaiseEvent(new CommentAddedEvent
        {
            Id = this.Id,
            Comment = comment,
            Username = username,
            CommentId = Guid.NewGuid(),
            CommentDate = DateTime.Now,
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        this.Id = @event.Id;
        this.comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void EditComment(string comment, string username, Guid commentId)
    {
        if (!this.active)
        {
            throw new InvalidOperationException("You cannot edit a comment of an inactive post!");
        }
        if (!this.comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to edit a comment that was made by another user");
        }

        this.RaiseEvent(new CommentUpdatedEvent
        {
            Id = this.Id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.Now,
        });
    }

    public void Aplly(CommentUpdatedEvent @event)
    {
        this.Id = @event.Id;
        this.comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!this.active)
        {
            throw new InvalidOperationException("You cannot remove a comment of an inactive post!");
        }
        if (!this.comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to remove a comment that was made by another user");
        }
        this.RaiseEvent(new CommentRemovedEvent
        {
            Id = this.Id,
            CommentId = commentId,
        });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        this.Id = @event.Id;
        this.comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!this.active)
        {
            throw new InvalidOperationException("The post has already been removed");
        }
        if (!this.author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
        {
            throw new InvalidOperationException("You are not allowed to delete a post that was made by someone else!");
        }

        this.RaiseEvent(new PostRemovedEvent
        {
            Id = this.Id,
        });
    }

    public void Apply(PostRemovedEvent @event)
    {
        this.Id = @event.Id;
        this.active = false;
    }
}