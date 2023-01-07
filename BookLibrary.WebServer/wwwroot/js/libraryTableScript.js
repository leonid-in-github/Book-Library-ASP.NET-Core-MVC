
$(document).ready(function () {  
    document.cookie = "TableSelectedMode=all";
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
                "bSortable": false,
                "mRender": function (data, type, row) {
                    return '<a href=\"Books/BookTrack?bookId=' + row[4] + '\" title="' + data + '">' + data + '</a>';
                }
            },
            {
                "sName": "AUTHORS",
                "bSortable": false
            },
            {
                "sName": "YEAR",
                "bSortable": true,
                "mRender": function (data, type, row) {
                    return new Date(data).getFullYear();
                },
                "width": "10%"
            },
            {
                "sName": "AVAILABILITY",
                "bSortable": true,
                "mRender": function (data, type, row) {
                    return data === "True" ? "Available" : "Not available";
                },
                "width": "15%"
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
                },
                "width": "15%"
            }

        ]
    });

}