using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.DataAccess;

public class DatabaseContextFactory
{
    private readonly Action<DbContextOptionsBuilder> configureDbContext;

    public DatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
    {
        this.configureDbContext = configureDbContext;
    }

    public DatabaseContext CreateDatabaseContext()
    {
        DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new();
        this.configureDbContext(optionsBuilder);

        return new DatabaseContext(optionsBuilder.Options);
    }
}