using Microsoft.EntityFrameworkCore;
using project_aurora.Models;

namespace project_aurora.Data;

public class AuroraDbContext : DbContext
{
    public AuroraDbContext(
        DbContextOptions<AuroraDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Challenge> Challenges => Set<Challenge>();

    public DbSet<CommunityPost> CommunityPosts => Set<CommunityPost>();
}