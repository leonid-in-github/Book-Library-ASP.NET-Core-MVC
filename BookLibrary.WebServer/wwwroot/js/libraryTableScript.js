
$(document).ready(function () {
    if (!getCookieValue("TableSelectedMode")) {
        document.cookie = "TableSelectedMode=all";
    }
    $("#SelectedMode").val(getCookieValue("TableSelectedMode"));
    LoadIndexBookTable();
    //document.cookie = 'TableSelectedMode=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
});

function getCookieValue(name) {
    const regex = new RegExp(`(^| )${name}=([^;]+)`)
    const match = document.cookie.match(regex)
    if (match) {
        return match[2]
    }
}

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
                "mRender": function (data, type, row) {
                    return data.join(", ");
                },
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
                        data + '\">Edit</a> | ' +
                        '<a href=\"Books/DeleteBook?bookId=' +
                        data +
                        '\" onClick=\"return confirm(\'Are you sure you want to delete this book?\');\">Delete</a>';
                },
                "width": "15%"
            }

        ]
    });

}