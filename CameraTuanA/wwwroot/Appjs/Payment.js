window.Payment = createCrudModule("Payment");

window.Payment = createCrudModule("Payment");

Payment.PaySuccess = function (res) {
    if (!res) {
        showToast('Không nhận được phản hồi từ máy chủ!', 'error');
        return;
    }

    if (res.success) {
        showToast(res.message, 'success');

        // Nếu là thanh toán COD (thanh toán khi nhận hàng)
        if (res.cod) {
            // Hiển thị thông báo thành công và chuyển hướng
            setTimeout(function () {
                window.location.href = "/Order/Success";
            }, 2000);
            return;
        }

        // Nếu có URL thanh toán (VNPay, Momo, v.v...)
        if (res.url) {
            setTimeout(function () {
                window.location.href = res.url;
            }, 1500);
            return;
        }

        // Trường hợp không có url và không cod (fallback)
        setTimeout(function () {
            window.location.href = "/";
        }, 1500);
    }
    else {
        // Trường hợp lỗi
        showToast(res.message || 'Tạo đơn hàng thất bại!', 'error');
    }
};

