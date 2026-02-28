using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("product_reviews")]
[Index("AccountId", Name = "idx_reviews_account")]
[Index("ProductId", Name = "idx_reviews_product")]
[Index("Status", Name = "idx_reviews_status")]
public partial class ProductReview
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("account_id")]
    public int AccountId { get; set; }

    [Column("order_id")]
    public int? OrderId { get; set; }

    [Column("rating_score")]
    public int RatingScore { get; set; }

    [Column("review_content", TypeName = "ntext")]
    public string? ReviewContent { get; set; }

    [Column("review_images")]
    public string? ReviewImages { get; set; }

    [Column("status")]
    [StringLength(20)]
    public string? Status { get; set; }

    [Column("reviewed_at")]
    public DateTime? ReviewedAt { get; set; }

    [Column("approved_at")]
    public DateTime? ApprovedAt { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("ProductReviews")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductReviews")]
    public virtual Product Product { get; set; } = null!;
}
