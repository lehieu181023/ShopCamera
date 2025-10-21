using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Account { get; set; }

    public virtual DbSet<Brand> Brand { get; set; }

    public virtual DbSet<Category> Category { get; set; }

    public virtual DbSet<Contact> Contact { get; set; }

    public virtual DbSet<Order> Order { get; set; }

    public virtual DbSet<OrderDetail> OrderDetail { get; set; }

    public virtual DbSet<PopularSearch> PopularSearch { get; set; }

    public virtual DbSet<Product> Product { get; set; }

    public virtual DbSet<ProductReview> ProductReview { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCart { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__accounts__3213E83F1D8B26E9");

            entity.ToTable("accounts", tb => tb.HasTrigger("trg_accounts_update"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValue("customer");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__brands__3213E83F26E1813F");

            entity.ToTable("brands", tb => tb.HasTrigger("trg_brands_update"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83F9E6947ED");

            entity.ToTable("categories", tb => tb.HasTrigger("trg_categories_update"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SortOrder).HasDefaultValue(0);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__contacts__3213E83F6A03F38E");

            entity.Property(e => e.ContactType).HasDefaultValue("other");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ProcessingStatus).HasDefaultValue("pending");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__orders__3213E83F118AB1BB");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PaymentMethod).HasDefaultValue("cod");
            entity.Property(e => e.ShippingFee).HasDefaultValue(0m);
            entity.Property(e => e.Status).HasDefaultValue(0);

            entity.HasOne(d => d.Account).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_orders_accounts");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__order_de__3213E83F6C42D398");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_details_orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_order_details_products");
        });

        modelBuilder.Entity<PopularSearch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__popular___3213E83F6E9394D5");

            entity.ToTable("popular_searches", tb => tb.HasTrigger("trg_searches_update"));

            entity.Property(e => e.FirstSearched).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastSearched).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SearchCount).HasDefaultValue(1);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__products__3213E83FE02A8677");

            entity.ToTable("products", tb => tb.HasTrigger("trg_products_update"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RatingCount).HasDefaultValue(0);
            entity.Property(e => e.RatingScore).HasDefaultValue(0m);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.StockQuantity).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_products_brands");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_products_categories");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__product___3213E83F33970F56");

            entity.Property(e => e.ReviewedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.Account).WithMany(p => p.ProductReviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reviews_accounts");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_reviews_products");
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__shopping__3213E83F8882F5E1");

            entity.ToTable("shopping_cart", tb => tb.HasTrigger("trg_cart_update"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Account).WithMany(p => p.ShoppingCarts).HasConstraintName("FK_cart_accounts");

            entity.HasOne(d => d.Product).WithMany(p => p.ShoppingCarts).HasConstraintName("FK_cart_products");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
