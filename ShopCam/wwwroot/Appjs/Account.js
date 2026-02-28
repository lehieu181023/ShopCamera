window.Account = {}; // khai báo object rỗng

Account.successAction = function (res, status, xhr) {
    UnBlockUI();
    if (res.success) {
        showToast(res.message, "success");
    } else {
        showToast(res.message, "error");
    }
};

Account.successLogin = function (res, status, xhr) {
    UnBlockUI();
    if (res.success) {
        showToast(res.message, "success");
        // đợi 1 tí để hiển thị toast rồi redirect
        setTimeout(function () {
            window.location.href = res.redirectUrl;
        }, 1000);
    } else {
        showToast(res.message, "error");
    }
};

