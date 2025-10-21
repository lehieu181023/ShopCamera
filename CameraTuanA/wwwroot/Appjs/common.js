function createCrudModule(controllerName) {

    function getSearchParams() {
        const data = {};
        $("#divSearch :input").each(function () {
            if (this.name && $(this).val() !== undefined) {
                data[this.name] = $(this).val();
            }
        });
        return data;
    }

    function loadData(targetSelector = "#listdata") {
        BlockUI('Đang tải...', targetSelector);
        const searchParams = getSearchParams();
        $.ajax({
            url: `/${controllerName}/ListData`,
            type: "Post",
            data: searchParams,
            success: function (response) {
                UnBlockUI( targetSelector);
                $(targetSelector).html(response);
            },
            error: xhr => handleAjaxError(xhr)
        });
    }

    function deleteData(id) {
        confirmDelete('Bạn có chắc chắn muốn xóa không?', function () {
            BlockUI();
            $.ajax({
                url: `/${controllerName}/Delete`,
                type: "POST",
                data: { id },
                success: function (res) {
                    UnBlockUI();
                    loadData();
                    showToast(res.message, res.success ? "success" : "error");
                },
                error: xhr => handleAjaxError(xhr)
            });
        });
    }

    function successAction(res, status, xhr) {
        UnBlockUI();
        if (res.success) {
            showToast(res.message, "success");
        } else {
            showToast(res.message, "error");
        }
    }

    function handleAjaxError(xhr) {
        UnBlockUI();
        if (xhr.status === 401) {
            showToast("Bạn không có quyền truy cập! Vui lòng đăng nhập.", "error");
        } else if (xhr.status === 403) {
            showToast("Bạn không có quyền thực hiện thao tác này!", "error");
        } else {
            showToast("Không thể tải dữ liệu!", "error");
        }
    }

    return {
        loadData,
        deleteData,
        successAction
    };
}

addToCart = function (id, quantity) {

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

loadPrice = function () {
    let spanPriceCart = $('#PriceCart');
    $.ajax({
        url: '/Cart/PriceCart',
        type: 'POST',
        success: function (res) {
            spanPriceCart.text(res);
        },
        error: function () {
            spanPriceCart.text("0 đ");
        }
    });
};
loadPrice();




