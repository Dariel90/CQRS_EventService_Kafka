using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IPostRepository postRepository;
        private readonly ICommentRepository commentRepository;

        public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
        }

        public async Task On(PostCreatedEvent @event)
        {
            var post = new PostEntity
            {
                PostId = @event.Id,
                Author = @event.Author,
                DatePosted = @event.DatePosted,
                Message = @event.Message,
            };
            await this.postRepository.CreateAsync(post);
        }

        public async Task On(MessageUpdatedEvent @event)
        {
            var post = await this.postRepository.GetByIdAsync(@event.Id);
            if (post == null)
            {
                return;
            }
            post.Message = @event.Message;
            await this.postRepository.UpdateAsync(post);
        }

        public async Task On(PostLikedEvent @event)
        {
            var post = await this.postRepository.GetByIdAsync(@event.Id);
            if (post == null)
            {
                return;
            }
            post.Likes++;
            await this.postRepository.UpdateAsync(post);
        }

        public async Task On(CommentAddedEvent @event)
        {
            var comment = new CommentEntity
            {
                Comment = @event.Comment,
                CommentDate = @event.CommentDate,
                CommentId = @event.CommentId,
                PostId = @event.Id,
                Username = @event.Username,
                Edited = false
            };
            await this.commentRepository.CreateAsync(comment);
        }

        public async Task On(CommentUpdatedEvent @event)
        {
            var comment = await this.commentRepository.GetByIdAsync(@event.Id);
            if (comment == null)
            {
                return;
            }
            comment.Comment = @event.Comment;
            comment.Edited = true;
            comment.CommentDate = @event.EditDate;
            await this.commentRepository.UpdateAsync(comment);
        }

        public async Task On(CommentRemovedEvent @event)
        {
            await this.commentRepository.DeleteAsync(@event.Id);
        }

        public async Task On(PostRemovedEvent @event)
        {
            await this.postRepository.DeleteAsync(@event.Id);
        }
    }
}