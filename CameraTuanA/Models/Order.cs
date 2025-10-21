using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

[Table("orders")]
[Index("OrderCode", Name = "UQ__orders__99D12D3F59F32765", IsUnique = true)]
[Index("AccountId", Name = "idx_orders_account")]
[Index("OrderDate", Name = "idx_orders_date")]
[Index("Status", Name = "idx_orders_status")]
public partial class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_code")]
    [StringLength(50)]
    public string OrderCode { get; set; } = null!;

    [Column("account_id")]
    public int AccountId { get; set; }

    [Column("recipient_name")]
    [StringLength(100)]
    public string RecipientName { get; set; } = null!;

    [Column("recipient_phone")]
    [StringLength(15)]
    public string RecipientPhone { get; set; } = null!;

    [Column("shipping_address", TypeName = "ntext")]
    public string ShippingAddress { get; set; } = null!;

    [Column("recipient_email")]
    [StringLength(100)]
    public string? RecipientEmail { get; set; }

    [Column("total_amount", TypeName = "decimal(12, 2)")]
    public decimal TotalAmount { get; set; }

    [Column("shipping_fee", TypeName = "decimal(10, 2)")]
    public decimal? ShippingFee { get; set; }

    [Column("final_total", TypeName = "decimal(12, 2)")]
    public decimal FinalTotal { get; set; }

    [Column("payment_method")]
    [StringLength(20)]
    public string? PaymentMethod { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Column("customer_notes", TypeName = "ntext")]
    public string? CustomerNotes { get; set; }

    [Column("admin_notes", TypeName = "ntext")]
    public string? AdminNotes { get; set; }

    [Column("order_date")]
    public DateTime OrderDate { get; set; }

    [Column("confirmed_date")]
    public DateTime? ConfirmedDate { get; set; }

    [Column("shipped_date")]
    public DateTime? ShippedDate { get; set; }

    [Column("completed_date")]
    public DateTime? CompletedDate { get; set; }

    [Column("processed_by")]
    public int? ProcessedBy { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("Orders")]
    public virtual Account Account { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
