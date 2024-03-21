$(() => {

    $.get('/home/DidLikeImage', { id: $("#image-id").val() }, function (didLike) {
        if (didLike) {
            $(".btn-primary").prop('disabled', true)
        }
    })

    setInterval(function () {
        getLikesForImage();
    }, 1000);

    $(".btn-primary").on('click', function () {
        $.post('/home/incrementlikes', { id: $("#image-id").val() }, function () {
            getLikesForImage()
        });
        $(this).prop('disabled', true)
    })

    function getLikesForImage() {
        $.get('/home/GetLikesById', { id: $("#image-id").val() }, function (likes) {
            $("#likes-count").text(likes)
        })
    }
});


