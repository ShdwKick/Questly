using DataModels.DTOs;

namespace DataModels;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<Authorization> Authorizations { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<AchievementCategory> AchievementCategories { get; set; }
    public DbSet<Place> Places { get; set; }
    public DbSet<PlaceType> PlaceTypes { get; set; }
    public DbSet<Leaderboard> Leaderboard { get; set; }
    public DbSet<Partner> Partners { get; set; }
    public DbSet<BlockUser> BlockUserHistory { get; set; }
        
    private readonly string _connectionString;
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.AvatarUrl).IsRequired(false);

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            
        });

        // Authorization
        modelBuilder.Entity<Authorization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.AuthToken).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        });

        // City
        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // Achievement
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Goal).HasDefaultValue(1);
           
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");

            entity.HasOne(e => e.City)
                .WithMany()
                .HasForeignKey(e => e.CityId);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId);
        });

        // AchievementCategory
        modelBuilder.Entity<AchievementCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        // UserAchievement
        modelBuilder.Entity<UserAchievement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.AchievementId).IsRequired();
            entity.Property(e => e.Progress).HasDefaultValue(0);
            entity.Property(e => e.EarnedAt).HasDefaultValueSql("now()");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.Achievement)
                .WithMany()
                .HasForeignKey(e => e.AchievementId);
        });

        // Place
        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.City)
                .WithMany()
                .HasForeignKey(e => e.CityId);

            entity.HasOne(e => e.Type)
                .WithMany()
                .HasForeignKey(e => e.TypeId);

            entity.HasOne(e => e.Achievement)
                .WithMany()
                .HasForeignKey(e => e.AchievementId);

            entity.HasOne(e => e.Partner)
                .WithMany()
                .HasForeignKey(e => e.PartnerId);
        });

        //PlaceType
        modelBuilder.Entity<PlaceType>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        //Leaderboard
        modelBuilder.Entity<Leaderboard>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            entity.HasOne(e => e.City)
                .WithMany()
                .HasForeignKey(e => e.CityId);
        });
        
        //BlockUserHistory
        modelBuilder.Entity<BlockUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        });

        base.OnModelCreating(modelBuilder);
    }
}