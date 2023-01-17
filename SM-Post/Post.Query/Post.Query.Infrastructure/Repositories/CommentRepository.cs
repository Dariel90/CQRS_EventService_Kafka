using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace Post.Query.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly DatabaseContextFactory contextFactory;

    public CommentRepository(DatabaseContextFactory contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    public async Task CreateAsync(CommentEntity comment)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        context.Comments.Add(comment);
        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        var comment = await GetByIdAsync(commentId);
        if (comment == null)
        {
            return;
        }
        context.Comments.Remove(comment);
        _ = await context.SaveChangesAsync();
    }

    public async Task<List<CommentEntity>> GetAllAsync()
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        return await context.Comments.ToListAsync();
    }

    public async Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        return await context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
    }

    public async Task UpdateAsync(CommentEntity comment)
    {
        using DatabaseContext context = this.contextFactory.CreateDatabaseContext();
        context.Comments.Update(comment);
        _ = await context.SaveChangesAsync();
    }
}