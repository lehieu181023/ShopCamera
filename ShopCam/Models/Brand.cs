using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("brands")]
[Index("Status", Name = "idx_brands_status")]
public partial class Brand
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("brand_name")]
    [StringLength(100)]
    public string BrandName { get; set; } = null!;

    [Column("description", TypeName = "ntext")]
    public string? Description { get; set; }

    [Column("logo")]
    [StringLength(255)]
    public string? Logo { get; set; }

    [Column("website")]
    [StringLength(255)]
    public string? Website { get; set; }

    [Column("country")]
    [StringLength(50)]
    public string? Country { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [InverseProperty("Brand")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
