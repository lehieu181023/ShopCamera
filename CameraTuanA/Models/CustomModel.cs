using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

public class LoginViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

public class reportCard
{
    public string filter { get; set; }
    public decimal quantity { get; set; }
    public bool isAsc { get; set; }
    public float percent { get; set; }
}

public class reportTopSell
{
    public string filter { get; set; }

    public List<ProductTopSell> listTopSell { get; set; }

}

public class ProductTopSell
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductImage{ get; set; }
    public int TotalQuantitySold { get; set; }

    public decimal PriceNow { get; set; }

    public decimal revenue { get; set; }
}
