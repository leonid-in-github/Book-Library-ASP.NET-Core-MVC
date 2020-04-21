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

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}