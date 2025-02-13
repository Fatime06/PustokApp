$(document).ready(function () {
    $("#imageInput").change(function (ev) {
        let file = ev.target.files[0];

        if (!file) {
            $("#imgPreview").hide();
            return;
        }

        var uploading = new FileReader();
        uploading.onload = function (displayimg) {
            $("#imgPreview").attr("src", displayimg.target.result).show();
        };
        uploading.readAsDataURL(file);
    });

    $(document).ready(function () {
        $(".deleteGenreButton").click(function (ev) {
            ev.preventDefault();
            let url = form.attr("href");

            Swal.fire({
                title: "Are you sure?",
                text: "You won't be able to revert this!",
                icon: "warning",
                showCancelButton: true,
                confirmButtonColor: "#3085d6",
                cancelButtonColor: "#d33",
                confirmButtonText: "Yes, delete it!"
            }).then((result) => {
                if (result.isConfirmed) {
                    fetch(url)
                        .then(response => {
                            if (response.ok) {
                                Swal.fire({
                                    title: "Deleted!",
                                    text: "The genre has been removed.",
                                    icon: "success"
                                });
                                window.location.reload();
                            } else {
                                Swal.fire({
                                    icon: "error",
                                    title: "Oops...",
                                    text: "Something went wrong. Try again.",
                                });
                            }
                        })
                        .catch(error => {
                            Swal.fire({
                                icon: "error",
                                title: "Error",
                                text: "Request failed. Please check your network.",
                            });
                        });
                }
            });
        });
    });

});
