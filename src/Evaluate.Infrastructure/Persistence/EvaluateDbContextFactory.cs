using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Evaluate.Infrastructure.Persistence;

public class EvaluateDbContextFactory : IDesignTimeDbContextFactory<EvaluateDbContext>
{
    public EvaluateDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EvaluateDbContext>();
        optionsBuilder.UseSqlite("Data Source=evaluate.db");
        return new EvaluateDbContext(optionsBuilder.Options);
    }
}
