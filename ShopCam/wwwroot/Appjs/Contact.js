window.Contact = {}; // khai báo object rỗng

Contact.successAction = function (res, status, xhr) {
    UnBlockUI();
    if (res.success) {
        showToast(res.message, "success");
        $('#FullName').val('');
        $('#Email').val('');
        $('#PhoneNumber').val('');
        $('#Subject').val('');
        $('#MessageContent').val('');
    } else {
        showToast(res.message, "error");
    }
};
