$(function () {
    $("#SelectedMode").change(function () {

        var value = $(this).val();

        document.cookie = "TableSelectedMode=" + value;

        ReloadIndexBookTable1();

        //delete_cookie("TableSelectedMode");
    });
});



function delete_cookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function ReloadIndexBookTable1() {
    
    $('#library-table').DataTable().ajax.reload();
}