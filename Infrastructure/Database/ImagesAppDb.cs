using Application.Abstractions.Data;
using Domain.Images;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed class ImagesAppDb : DbContext, IApplicationDbContext
{
    public DbSet<Image> Images { get; set; }
    public DbSet<User> Users { get; set; }
    public ImagesAppDb(DbContextOptions<ImagesAppDb> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        SeedingDatabase(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ImagesAppDb).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }

    private static void SeedingDatabase(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = new Guid("132dc49d-5925-463e-bb39-d86e26cdf6d4"),
            Email = "admin@ImageApp.com",
            FirstName = "Admin",
            LastName = "User",
            PasswordHash = "adminhash",
            PasswordSalt = "adminsalt",
            UserRole = UserRoles.Admin,
            IsActive = true
        });
    }
}
