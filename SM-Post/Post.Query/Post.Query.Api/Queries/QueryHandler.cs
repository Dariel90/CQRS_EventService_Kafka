using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries;

public class QueryHandler : IQueryHandler
{
    private readonly IPostRepository postRepository;

    public QueryHandler(IPostRepository repository)
    {
        this.postRepository = repository;
    }

    public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
    {
        return await this.postRepository.GetAllAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query)
    {
        var post = await this.postRepository.GetByIdAsync(query.Id);
        return new List<PostEntity> { post };
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsByAuthorQuery query)
    {
        return await this.postRepository.GetByAuthorAsync(query.Author);
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithCommentsQuery query)
    {
        return await this.postRepository.ListWithCommentAsync();
    }

    public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query)
    {
        return await this.postRepository.ListWithLikesAsync(query.NumberOfLikes);
    }
}