using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("shopping_cart")]
[Index("AccountId", Name = "idx_cart_account")]
[Index("ProductId", Name = "idx_cart_product")]
public partial class ShoppingCart
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("account_id")]
    public int AccountId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("ShoppingCarts")]
    public virtual Account Account { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ShoppingCarts")]
    public virtual Product Product { get; set; } = null!;
}
