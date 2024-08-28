using Microsoft.EntityFrameworkCore;
using Tech.Challenge.I.Domain.Entities;

namespace Tech.Challenge.I.Infrastructure.RepositoryAccess;
public class TechChallengeContext(DbContextOptions<TechChallengeContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RegionDDD> DDDRegions { get; set; }
    public DbSet<Contact> Contacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TechChallengeContext).Assembly);
    }
}