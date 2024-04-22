$(function () {
    $("#SelectedMode").change(function () {
        var value = $(this).val();

        document.cookie = "TableSelectedMode=" + value;

        ReloadIndexBookTable1();

        //deleteCookie("TableSelectedMode");
    });

});

function ReloadIndexBookTable1() {
    
    $('#library-table').DataTable().ajax.reload();
}

