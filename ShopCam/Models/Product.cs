using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("products")]
[Index("ProductCode", Name = "UQ__products__AE1A8CC48899F599", IsUnique = true)]
[Index("BrandId", Name = "idx_products_brand")]
[Index("CategoryId", Name = "idx_products_category")]
[Index("SellingPrice", Name = "idx_products_price")]
[Index("RatingScore", Name = "idx_products_rating", AllDescending = true)]
[Index("Status", Name = "idx_products_status")]
public partial class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_name")]
    [StringLength(200)]
    public string ProductName { get; set; } = null!;

    [Column("product_code")]
    [StringLength(50)]
    public string ProductCode { get; set; } = null!;

    [Column("short_description", TypeName = "ntext")]
    public string? ShortDescription { get; set; }

    [Column("detailed_description", TypeName = "ntext")]
    public string? DetailedDescription { get; set; }

    [Column("selling_price", TypeName = "decimal(12, 2)")]
    public decimal SellingPrice { get; set; }

    [Column("original_price", TypeName = "decimal(12, 2)")]
    public decimal? OriginalPrice { get; set; }

    [Column("stock_quantity")]
    public int? StockQuantity { get; set; }

    [Column("main_image")]
    [StringLength(255)]
    public string? MainImage { get; set; }

    [Column("image_gallery")]
    public string? ImageGallery { get; set; }

    [Column("technical_specs")]
    public string? TechnicalSpecs { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("brand_id")]
    public int BrandId { get; set; }

    [Column("view_count")]
    public int? ViewCount { get; set; }

    [Column("rating_score", TypeName = "decimal(2, 1)")]
    public decimal? RatingScore { get; set; }

    [Column("rating_count")]
    public int? RatingCount { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand Brand { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    [InverseProperty("Product")]
    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
