using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("order_details")]
[Index("OrderId", Name = "idx_order_details_order")]
[Index("ProductId", Name = "idx_order_details_product")]
public partial class OrderDetail
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_name")]
    [StringLength(200)]
    public string ProductName { get; set; } = null!;

    [Column("product_image")]
    [StringLength(255)]
    public string? ProductImage { get; set; }

    [Column("selling_price", TypeName = "decimal(12, 2)")]
    public decimal SellingPrice { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("subtotal", TypeName = "decimal(12, 2)")]
    public decimal Subtotal { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderDetails")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderDetails")]
    public virtual Product Product { get; set; } = null!;
}
