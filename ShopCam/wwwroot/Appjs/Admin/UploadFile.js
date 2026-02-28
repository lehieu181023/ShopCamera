
UploadFile = function(input,name) {
    // Kiểm tra file
    if (input.files && input.files[0]) {
        let file = input.files[0];
        let formData = new FormData();
        formData.append("file", file);

        // Sử dụng jQuery AJAX
        $.ajax({
            url: '/Admin/Upload/UploadFile', // URL đến file PHP xử lý
            type: 'POST',
            data: formData,
            contentType: false, // Không đặt Content-Type
            processData: false, // Không xử lý dữ liệu
            xhr: function () {
                let xhr = new window.XMLHttpRequest();
                // Theo dõi tiến trình upload
                xhr.upload.addEventListener('progress', function (e) {
                    if (e.lengthComputable) {
                        let percentComplete = Math.round((e.loaded / e.total) * 100);
                        $('#progress-container-'+name).show();
                        $('#progress-bar-'+name).css('width', percentComplete + '%').text(percentComplete + '%');
                    }
                }, false);
                return xhr;
            },
            success: function (response) {
                if (response.success) {
                    console.log("thêm file: " +response.filePath); // Đường dẫn file đã tải lên
                    $('#imgfile-'+name).attr('src', response.filePath); // Hiển thị ảnh
                    $('#Preview-'+name).removeAttr('hidden');
                    $('#imgfile-value-'+name).val(response.filePath); // Gán đường dẫn file vào input hidden
                    $('#progress-bar-'+name).css('width', '0%').text('0%'); // Reset tiến trình
                    $('#progress-container-'+name).hide();
                } else {
                    alert(response.message); // Hiển thị lỗi
                }           
            },
            error: function () {
                alert('Có lỗi xảy ra khi gửi yêu cầu!');
            }
        });
    } else {
        alert('Vui lòng chọn file!');
    }
}

deletefile = function(input,name) {
    $.ajax({
        url: '/Admin/Upload/DeleteFile', // URL đến file PHP xử lý
        type: 'POST',
        data: { fileName: input},
        success: function (response) {
            if (response.success) {
                console.log("xóa file: " + response.filePath);
                $('#imgfile-' + name).attr('src', 'https://img.freepik.com/premium-vector/default-image-icon-vector-missing-picture-page-website-design-mobile-app-no-photo-available_87543-11093.jpg');
                $('#imgfile-value-' + name).val('https://img.freepik.com/premium-vector/default-image-icon-vector-missing-picture-page-website-design-mobile-app-no-photo-available_87543-11093.jpg');
            } else {
                alert(response.message); // Hiển thị lỗi
            }           
        },
        error: function () {
            alert('Có lỗi xảy ra khi gửi yêu cầu!');
        }
    });
}


