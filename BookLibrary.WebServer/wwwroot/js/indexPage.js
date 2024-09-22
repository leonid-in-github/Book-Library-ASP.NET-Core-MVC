$(function () {
    $("#SelectedMode").on("change", function () {
        var value = $(this).val();
        document.cookie = `TableSelectedMode=${value}`;
        ReloadIndexBookTable1();
    });

});

function ReloadIndexBookTable1() {
    
    $('#library-table').DataTable().ajax.reload();
}

