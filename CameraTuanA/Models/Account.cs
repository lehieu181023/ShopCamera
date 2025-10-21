using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("accounts")]
[Index("Email", Name = "UQ__accounts__AB6E6164107CBFEC", IsUnique = true)]
[Index("Username", Name = "UQ__accounts__F3DBC572845D03FF", IsUnique = true)]
[Index("Role", Name = "idx_accounts_role")]
[Index("Status", Name = "idx_accounts_status")]
public partial class Account
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(255)]
    public string Password { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("full_name")]
    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Column("phone_number")]
    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [Column("address", TypeName = "ntext")]
    public string? Address { get; set; }

    [Column("role")]
    [StringLength(20)]
    public string? Role { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [InverseProperty("Account")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Account")]
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

    [InverseProperty("Account")]
    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
