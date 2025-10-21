window.Cart = createCrudModule("Cart");

Cart.loadData();

Cart.addToCart = function (id, quantity) {

    if (quantity === null || quantity === undefined || quantity === '') {
        quantity = 1;
    }
    let q = parseInt(quantity);
    if (isNaN(q)) {
        q = 1;
    }
    if (!id) {
        showToast("Không xác định được sản phẩm.", "error");
        return;
    }
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        data: { Id: id, Quantity: q },
        success: function (res) {
            if (res.success) {
                showToast(res.message, 'success');
            } else {
                showToast(res.message, 'error');
            }
            Cart.loadData();
        },
        error: function (xhr) {
            if (xhr.status === 401) {
                showToast("Vui lòng đăng nhập.", "error");
            } else {
                showToast("Lỗi không xác định!", "error");
            }
        }
    });
};
