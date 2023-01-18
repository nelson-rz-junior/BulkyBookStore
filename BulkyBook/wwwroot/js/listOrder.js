var dataTable;

$(document).ready(function () {
    var url = window.location.search;

    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else if (url.includes("pending")) {
        loadDataTable("pending");
    }
    else if (url.includes("completed")) {
        loadDataTable("completed");
    }
    else if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else {
        loadDataTable("all");
    }
});

function loadDataTable(status) {
    dataTable = $("#orders").DataTable({
        "ajax": {
            "url": `/api/orders/${status}`
        },
        "columnDefs": [
            { targets: [5], className: 'dt-body-right' }
        ],
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "25%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "20%" },
            { "data": "orderStatus", "width": "15%" },
            {
                "data": "orderTotal",
                "render": $.fn.dataTable.render.number('.', ',', 2, 'R$ '),
                "width": "15%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Order/Details?orderId=${data}" class="btn-sm btn-primary mx-2" title="Details" style="text-decoration:none;">
                                    <i class="bi bi-pencil-square"></i>
                                </a>
                            </div>`;
                },
                "width": "5%"
            }
        ],
        "language": {
            "emptyTable": "No data found."
        },
        "width": "100%"
    });
}
