using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("categories")]
[Index("SortOrder", Name = "idx_categories_sort_order")]
[Index("Status", Name = "idx_categories_status")]
public partial class Category
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("category_name")]
    [StringLength(100)]
    public string CategoryName { get; set; } = null!;

    [Column("description", TypeName = "ntext")]
    public string? Description { get; set; }

    [Column("image")]
    [StringLength(255)]
    public string? Image { get; set; }

    [Column("sort_order")]
    public int? SortOrder { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
