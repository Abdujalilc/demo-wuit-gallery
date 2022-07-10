$('#upload-image').on('click', function () {
  var url = $(this).data('url');
  $.get(url).done(function (data) {
    $('.modal-content').html(data);
    $('#modal-container').modal('show');
  });
});

$('.toggle-gallery').on('click', function () {
  var url = $(this).data('url');
  $.get(url).done(function (data) {
    $('.modal-content').html(data);
    $('#modal-container').modal('show');
  });
});
