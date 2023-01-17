using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory contextFactory;

    public PostRepository(DatabaseContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async Task CreateAsync(PostEntity post)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        context.Posts.Add(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        var post = await GetByIdAsync(postId);

        if (post == null)
        {
            return;
        }
        context.Posts.Remove(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task<List<PostEntity>> GetAllAsync()
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        return await context.Posts.AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PostEntity>> GetByAuthorAsync(string author)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        return await context.Posts.AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .Where(p => p.Author.Contains(author))
            .ToListAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        return await context.Posts.Include(p => p.Comments).FirstOrDefaultAsync(p => p.PostId == postId);
    }

    public async Task<List<PostEntity>> ListWithCommentAsync()
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        return await context.Posts.AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .Where(p => p.Comments != null && p.Comments.Any())
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        return await context.Posts.AsNoTracking()
            .Include(p => p.Comments).AsNoTracking()
            .Where(p => p.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        context.Posts.Update(post);
        _ = await context.SaveChangesAsync();
    }
}