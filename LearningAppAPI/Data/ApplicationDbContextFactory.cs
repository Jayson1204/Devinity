using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LearningApp.Api.Data;

namespace LearningApp.Api.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=learningapp;Uid=root;Pwd=;",
            new MySqlServerVersion(new Version(8, 0, 0))
        );

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}