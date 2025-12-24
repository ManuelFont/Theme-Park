using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;
public static class SqlContextFactory
{
    public static ParqueDbContext CreateMemoryContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ParqueDbContext>();
        optionsBuilder.UseInMemoryDatabase("TestDB");
        var context = new ParqueDbContext(optionsBuilder.Options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return context;
    }
}
