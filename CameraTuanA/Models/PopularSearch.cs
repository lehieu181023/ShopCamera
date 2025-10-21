using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("popular_searches")]
[Index("AccountId", Name = "idx_searches_account")]
[Index("SearchCount", Name = "idx_searches_count", AllDescending = true)]
[Index("SearchKeyword", Name = "idx_searches_keyword")]
public partial class PopularSearch
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("search_keyword")]
    [StringLength(200)]
    public string SearchKeyword { get; set; } = null!;

    [Column("search_count")]
    public int? SearchCount { get; set; }

    [Column("account_id")]
    public int? AccountId { get; set; }

    [Column("ip_address")]
    [StringLength(45)]
    public string? IpAddress { get; set; }

    [Column("first_searched")]
    public DateTime? FirstSearched { get; set; }

    [Column("last_searched")]
    public DateTime? LastSearched { get; set; }
}
