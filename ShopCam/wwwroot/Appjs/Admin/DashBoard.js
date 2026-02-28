
customercard = function (filter) {
    $.ajax({
        url: "/Admin/Dashboard/customercard", // Gọi file xử lý
        type: "POST",
        data: { filter: filter }, // Gửi tham số filter đến file
        success: function (response) {
            $("#customer-card").html(response); // Chèn dữ liệu vào bảng
        },
        error: function () {
            showToast("Không thể tải dữ liệu!","error");
        }
    });
}
recentsales = function () {
    $.ajax({
        url: "/Admin/Dashboard/recentsales", // Gọi file xử lý
        type: "POST",
        success: function (response) {
            $("#recentSales").html(response); // Chèn dữ liệu vào bảng
        },
        error: function () {
            showToast("Không thể tải dữ liệu!", "error");
        }
    });
}
report = function () {
    $.ajax({
        url: "/Admin/Dashboard/report", // Gọi file xử lý
        type: "POST",
        success: function (response) {
            $("#report").html(response); // Chèn dữ liệu vào bảng
            gen();
        },
        error: function () {
            showToast("Không thể tải dữ liệu!", "error");
        }
    });
}
revenuecard = function (filter) {
    $.ajax({
        url: "/Admin/Dashboard/revenuecard", // Gọi file xử lý
        type: "POST",
        data: { filter: filter }, // Gửi tham số filter đến file
        success: function (response) {
            $("#revenue-card").html(response); // Chèn dữ liệu vào bảng
        },
        error: function () {
            showToast("Không thể tải dữ liệu!", "error");
        }
    });
}
salecard = function (filter) {
    $.ajax({
        url: "/Admin/Dashboard/salecard", // Gọi file xử lý
        type: "POST",
        data: { filter: filter }, // Gửi tham số filter đến file
        success: function (response) {
            $("#sales-card").html(response); // Chèn dữ liệu vào bảng
        },
        error: function () {
            showToast("Không thể tải dữ liệu!", "error");
        }
    });
}
TopSelling = function (filter) {
    $.ajax({
        url: "/Admin/Dashboard/TopSelling", // Gọi file xử lý
        type: "POST",
        data: { filter: filter }, // Gửi tham số filter đến file
        success: function (response) {
            $("#topSelling").html(response); // Chèn dữ liệu vào bảng
        },
        error: function () {
            showToast("Không thể tải dữ liệu!", "error");
        }
    });
}
customercard();

recentsales();

revenuecard();

salecard();

TopSelling();

// Thêm các hàm tương tự cho các bảng khác



