using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("contacts")]
[Index("ProcessingStatus", Name = "idx_contacts_status")]
[Index("ContactType", Name = "idx_contacts_type")]
public partial class Contact
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("full_name")]
    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("phone_number")]
    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [Column("subject")]
    [StringLength(200)]
    public string Subject { get; set; } = null!;

    [Column("message_content", TypeName = "ntext")]
    public string MessageContent { get; set; } = null!;

    [Column("contact_type")]
    [StringLength(20)]
    public string? ContactType { get; set; }

    [Column("processing_status")]
    [StringLength(20)]
    public string? ProcessingStatus { get; set; }

    [Column("admin_notes", TypeName = "ntext")]
    public string? AdminNotes { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("processed_at")]
    public DateTime? ProcessedAt { get; set; }

    [Column("processed_by")]
    public int? ProcessedBy { get; set; }
}
