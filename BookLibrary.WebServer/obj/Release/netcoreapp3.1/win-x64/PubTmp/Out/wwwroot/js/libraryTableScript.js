
$(document).ready(function () {  
    document.cookie = "TableSelectedMode=1";
    LoadIndexBookTable();
    //document.cookie = 'TableSelectedMode=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
});

function LoadIndexBookTable() {
    
    $('#library-table').DataTable({
        "bServerSide": true,
        "sAjaxSource": "Books/MainBooksTableAjaxHandler",
        "bProcessing": true,
        "aoColumns": [
            {
                "sName": "NAME",
                "bSortable": true,
                "mRender": function (data, type, row) {
                    return '<a href=\"Books/BookTrack?bookId=' + row[4] + '\">' + data + '</a>';
                }
            },
            {
                "sName": "AUTHORS",
                "bSortable": true
            },
            {
                "sName": "YEAR",
                "bSortable": true
            },
            {
                "sName": "AVAILABILITY",
                "bSortable": true
            },
            {
                "sName": "ID",
                "bSearchable": false,
                "bSortable": false,
                "mRender": function (data, type, row) {
                    return '<a href=\"Books/EditBook?bookId=' +
                        data + '\">Edit</a> |' +
                        '<a href=\"Books/DeleteBook?bookId=' +
                        data +
                        '\" onClick=\"return confirm(\'Are you sure you want to delete this book?\');\">Delete</a>';
                }
            }

        ]
    });

}