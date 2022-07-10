$('#upload_image').on('click', function () {
  var fileUpload = $("#files").get(0);
  var parentId = $("#PhotoGalleryId").val();
  var files = fileUpload.files;

  var fileData = new FormData();
  for (var value of fileData) {
    fileData.append(value);
  }

  $.ajax({
    url: "/Images/UploadImage",
    type: "POST",
    data: fileData,
    cache: false,
    contentType: false,
    processData: false,
    async: false,
    success: function (data) {
      // do something here
    },
    error: function (err) {
      alert(err.statusText);
    }
  });
});