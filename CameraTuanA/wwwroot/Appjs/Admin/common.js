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
        BlockUI();
        const searchParams = getSearchParams();
        $.ajax({
            url: `/Admin/${controllerName}/ListData`,
            type: "Post",
            data: searchParams,
            success: function (response) {
                UnBlockUI();
                $(targetSelector).html(response);
            },
            error: xhr => handleAjaxError(xhr)
        });
    }

    function loadModelAdd(targetSelector = "#target-div", modalSelector = "#myModal") {
        BlockUI();
        $.ajax({
            url: `/Admin/${controllerName}/Create`,
            type: "GET",
            success: function (response) {
                UnBlockUI();
                $(targetSelector).html(response);
                $(modalSelector).modal("show");
                initSelect2();
            },
            error: xhr => handleAjaxError(xhr)
        });
    }

    function loadModelEdit(id, targetSelector = "#target-div", modalSelector = "#myModal") {
        BlockUI();
        $.ajax({
            url: `/Admin/${controllerName}/Edit`,
            type: "GET",
            data: { id },
            success: function (response) {
                UnBlockUI();
                $(targetSelector).html(response);
                $(modalSelector).modal("show");
                initSelect2();
            },
            error: xhr => handleAjaxError(xhr)
        });
    }

    function loadModelDetail(id, targetSelector = "#target-div", modalSelector = "#myModal") {
        BlockUI();
        $.ajax({
            url: `/Admin/${controllerName}/Detail`,
            type: "GET",
            data: { id },
            success: function (response) {
                loadData();
                UnBlockUI();
                $(targetSelector).html(response);
                $(modalSelector).modal("show");
            },
            error: xhr => handleAjaxError(xhr)
        });
    }

    function deleteData(id) {
        confirmDelete('Bạn có chắc chắn muốn xóa không?', function () {
            BlockUI();
            $.ajax({
                url: `/Admin/${controllerName}/Delete`,
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

    function editStatus(id) {
        BlockUI();
        $.ajax({
            url: `/Admin/${controllerName}/Status`,
            type: "POST",
            data: { id },
            success: function (res) {
                UnBlockUI();
                loadData();
                showToast(res.message, res.success ? "success" : "error");
            },
            error: xhr => handleAjaxError(xhr)
        });
    }

    function successAction(res, status, xhr) {
        UnBlockUI();
        if (res.success) {
            $('#btnclosemodel').click();
            loadData();
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
        loadModelAdd,
        loadModelEdit,
        loadModelDetail,
        deleteData,
        editStatus,
        successAction,
        handleAjaxError
    };
}

