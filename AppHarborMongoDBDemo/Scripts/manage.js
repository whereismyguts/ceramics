$(document).ready(function () {
    $('.image-container').slick();

    var settings = {
        url: "/Manage/RememberImage",
        method: "POST",
        allowedTypes: "jpg,png,gif", //doc,pdf,zip
        fileName: "myfile",
        multiple: true,
        onSuccess: function (files, data, xhr) {
            $("#status").html("<font color='green'>Upload is success</font>");
            $("#mulitplefileuploader").append('<div class="thumbnail old-image"> <img src="data:image/png;base64, ' + data + '"></div>')

            //$("#status").html(' <div class="grid"> <img src="data:image/png;base64, ' + data + '"></div>');
        },
        onError: function (files, status, errMsg) {
            $("#status").html("<font color='red'>Upload is Failed</font>");
        }
    }

    $("#mulitplefileuploader").uploadFile(settings);
});

function removeImage(id, elem) {
    $.ajax({
        type: "POST",
        url: "Manage/RemoveImage",
        data: { id: id },
        success: function (res) {
            if (res === "removed") {
                elem.remove();
            }
        },
        datatype: "json",
        traditional: true
    });
}
