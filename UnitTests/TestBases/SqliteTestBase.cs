using Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.TestBases;

public abstract class SqliteTestBase : IAsyncDisposable
{
    protected readonly CollegeDbContext Context;
    private readonly SqliteConnection _connection;

    protected SqliteTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    
        var options = new DbContextOptionsBuilder<CollegeDbContext>()
            .UseSqlite(_connection)
            .Options;
    
        Context = new CollegeDbContext(options);
        Context.Database.EnsureCreated();
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await Context.Database.EnsureDeletedAsync();
        await _connection.DisposeAsync();
        await _connection.CloseAsync();
    }
}