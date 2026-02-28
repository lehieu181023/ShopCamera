using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Models;

public class EnumMethod
{
    public enum OrderStatus
    {
        Pending = 0,
        Processing = 1,
        Shipped = 2,
        Complete = 3,
        CancellRequest = 4,
        Cancelled = 5,

    }

    public static string OrderStatusToString(int status)
    {
        return status switch
        {
            0 => "Đang chờ sử lý",
            1 => "Đã thanh toán",
            2 => "Đang vận chuyển",
            3 => "Hoàn thành",
            4 => "Yêu cầu hủy",
            5 => "Đã hủy",
            _ => "Unknown"
        };
    }

    public enum PaymentMethod
    {
        cod = 0,
        bank_transfer = 1,
        VNPay = 2
    }

    public static string PaymentMethodToString(int method)
    {
        return method switch
        {
            0 => "Thanh toán khi nhận hàng",
            1 => "Chuyển khoản ngân hàng",
            2 => "Thanh toán VNPay",
            _ => "Unknown"
        };
    }

}

