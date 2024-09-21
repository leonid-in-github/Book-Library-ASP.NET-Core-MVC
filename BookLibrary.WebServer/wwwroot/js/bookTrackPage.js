$(function () {
    if (getCookie("BookTrackTableSelectedMode")) {
        $("#SelectedMode")[0].value = getCookie("BookTrackTableSelectedMode");
    }

    if (document.getElementById("track-table") == null || document.getElementById("track-table").rows.length <= 10) {
        document.getElementById("SelectedMode").style.display = "none";
    }

    $("#SelectedMode").change(function () {

        var value = $(this).val();

        document.cookie = "BookTrackTableSelectedMode=" + value;
        location.reload();

    });
});

