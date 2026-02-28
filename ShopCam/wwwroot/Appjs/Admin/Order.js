window.Order = createCrudModule("Order");

Order.loadData();

Order.changeStatus = function (id, newStatus) {
    BlockUI();
    $.ajax({
        url: `/Admin/order/changeStatus`,
        type: "POST",
        data: { id: id, Status: newStatus },
        success: function (res) {
            UnBlockUI();
            Order.loadData();
            showToast(res.message, res.success ? "success" : "error");
        },
        error: xhr => Order.handleAjaxError(xhr)
    });
}