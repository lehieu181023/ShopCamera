let selectedStar = 0;

// chọn sao
$(document).on('click', '#starRating i', function () {
    selectedStar = parseInt($(this).data('star'));
    $('#starRating i').each(function () {
        const star = $(this).data('star');
        $(this).toggleClass('text-warning', star <= selectedStar);
        $(this).toggleClass('text-muted', star > selectedStar);
    });
});

// gửi form
$('#btnSubmitReview').on('click', function () {
    const form = $('#reviewForm');
    const data = {
        product_id: @Model.Id, // gắn id sản phẩm
        FullName: form.find('input[name="FullName"]').val(),
        Email: form.find('input[name="Email"]').val(),
        Content: form.find('textarea[name="Content"]').val(),
        Rating: selectedStar
    };

    if (data.Rating === 0) {
        alert("Vui lòng chọn số sao!");
        return;
    }

    $.ajax({
        url: '/Review/Create',
        type: 'POST',
        data: data,
        success: function (res) {
            if (res.success) {
                alert("Cảm ơn bạn đã gửi đánh giá!");
                form.trigger('reset');
                selectedStar = 0;
                $('#starRating i').removeClass('text-warning').addClass('text-muted');
            } else {
                alert(res.message || "Có lỗi xảy ra!");
            }
        },
        error: function () {
            alert("Không thể gửi đánh giá. Vui lòng thử lại!");
        }
    });
});

