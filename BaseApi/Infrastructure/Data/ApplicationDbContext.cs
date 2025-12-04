using BaseApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<SocialMediaLink> SocialMediaLinks { get; set; }
        public DbSet<TokenBlacklist> TokenBlacklists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
            });

            // Menu Configuration
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Slug).HasMaxLength(150);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Url).HasMaxLength(500);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(500);

                // Self-referencing relationship for parent-child
                entity.HasOne(e => e.Parent)
                      .WithMany(e => e.SubMenus)
                      .HasForeignKey(e => e.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // User relationship
                entity.HasOne(e => e.Creator)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                // Enum conversions
                entity.Property(e => e.MenuType)
                      .HasConversion<int>();

                entity.Property(e => e.LinkType)
                      .HasConversion<int>();
            });

            // Slider Configuration
            modelBuilder.Entity<Slider>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.Subtitle).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.MobileImageUrl).HasMaxLength(1000);
                entity.Property(e => e.LinkUrl).HasMaxLength(500);
                entity.Property(e => e.ButtonText).HasMaxLength(100);
                entity.Property(e => e.TargetLocation).HasMaxLength(100);

                // Indexes for performance
                entity.HasIndex(e => e.SliderType);
                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.TargetLocation);
                entity.HasIndex(e => new { e.SliderType, e.IsActive, e.Order });

                // User relationship
                entity.HasOne(e => e.Creator)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                // Enum conversions
                entity.Property(e => e.SliderType)
                      .HasConversion<int>();

                entity.Property(e => e.LinkType)
                      .HasConversion<int>();
            });

            // Product Configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.ProductCode).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Slug).HasMaxLength(300);
                entity.Property(e => e.ShortDescription).HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(5000);
                entity.Property(e => e.ProductCode).HasMaxLength(100);
                entity.Property(e => e.MainImageUrl).HasMaxLength(1000);
                entity.Property(e => e.ImageUrls).HasMaxLength(5000);
                entity.Property(e => e.Currency).HasMaxLength(3);
                entity.Property(e => e.Dimensions).HasMaxLength(1000);
                entity.Property(e => e.Material).HasMaxLength(200);
                entity.Property(e => e.Colors).HasMaxLength(200);
                entity.Property(e => e.Features).HasMaxLength(3000);
                entity.Property(e => e.MetaTitle).HasMaxLength(200);
                entity.Property(e => e.MetaDescription).HasMaxLength(500);
                entity.Property(e => e.Keywords).HasMaxLength(500);

                // Indexes for performance
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsFeatured);
                entity.HasIndex(e => e.IsNewProduct);
                entity.HasIndex(e => new { e.Category, e.Status, e.Order });
                entity.HasIndex(e => new { e.Status, e.IsFeatured });
                entity.HasIndex(e => new { e.Status, e.IsNewProduct });

                // User relationship
                entity.HasOne(e => e.Creator)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                // Enum conversions
                entity.Property(e => e.Category)
                      .HasConversion<int>();

                entity.Property(e => e.Status)
                      .HasConversion<int>();

                // Decimal precision
                entity.Property(e => e.Price)
                      .HasColumnType("decimal(18,2)");

                entity.Property(e => e.DiscountPrice)
                      .HasColumnType("decimal(18,2)");
            });

            // Page Configuration
            modelBuilder.Entity<Page>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Summary).HasMaxLength(1000);
                entity.Property(e => e.FeaturedImageUrl).HasMaxLength(1000);
                entity.Property(e => e.MetaTitle).HasMaxLength(200);
                entity.Property(e => e.MetaDescription).HasMaxLength(500);
                entity.Property(e => e.Keywords).HasMaxLength(500);
                entity.Property(e => e.CanonicalUrl).HasMaxLength(500);

                // Indexes for performance
                entity.HasIndex(e => e.Template);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.IsHomePage);
                entity.HasIndex(e => e.Visibility);
                entity.HasIndex(e => new { e.Status, e.IsHomePage });
                entity.HasIndex(e => new { e.Status, e.Template });

                // User relationships
                entity.HasOne(e => e.Creator)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                // Enum conversions
                entity.Property(e => e.Template).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Visibility).HasConversion<int>();
            });

            // SocialMediaLink Configuration
            modelBuilder.Entity<SocialMediaLink>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(1000);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ColorCode).HasMaxLength(7);

                // Indexes

                entity.HasIndex(e => e.IsActive);

                // User relationships
                entity.HasOne(e => e.Creator)
                      .WithMany()
                      .HasForeignKey(e => e.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Updater)
                      .WithMany()
                      .HasForeignKey(e => e.UpdatedBy)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            // TokenBlacklist Configuration
            modelBuilder.Entity<TokenBlacklist>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
                entity.HasIndex(e => e.Token);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ExpiresAt);
                entity.HasIndex(e => new { e.Token, e.ExpiresAt });

                // User relationship
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}