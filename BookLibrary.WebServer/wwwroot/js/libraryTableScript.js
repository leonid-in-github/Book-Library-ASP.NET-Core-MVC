
$(function () {
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
        serverSide: true,
        ajax: "Books/BooksTableAjaxHandler",
        processing: true,
        columns: [
            {
                name: "NAME",
                sortable: false,
                render: function (data, type, row) {
                    return '<a href=\"Books/BookTrack?bookId=' + row[4] + '\" title="' + data + '">' + data + '</a>';
                }
            },
            {
                name: "AUTHORS",
                render: function (data, type, row) {
                    return data.join(", ");
                },
                sortable: false
            },
            {
                name: "YEAR",
                sortable: true,
                render: function (data, type, row) {
                    return new Date(data).getFullYear();
                },
                width: "10%"
            },
            {
                name: "AVAILABILITY",
                sortable: true,
                render: function (data, type, row) {
                    return data === "True" ? "Available" : "Not available";
                },
                width: "15%"
            },
            {
                name: "ID",
                searchable: false,
                sortable: false,
                render: function (data, type, row) {
                    return '<a href=\"Books/EditBook?bookId=' +
                        data + '\">Edit</a> | ' +
                        '<a href=\"Books/DeleteBook?bookId=' +
                        data +
                        '\" onClick=\"return confirm(\'Are you sure you want to delete this book?\');\">Delete</a>';
                },
                width: "15%"
            }

        ],
        order: []
    });

}