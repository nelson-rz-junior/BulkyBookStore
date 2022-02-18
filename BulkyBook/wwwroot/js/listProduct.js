var dataTable;

function loadDataTable() {
    dataTable = $("#products").DataTable({
        "ajax": {
            "url": "/api/products"
        },
        "columnDefs": [
            { targets: [5], className: 'dt-body-right' }
        ],
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "title", "width": "30%" },
            { "data": "isbn", "width": "15%" },
            { "data": "author", "width": "20%" },
            { "data": "categoryName", "width": "10%" },
            {
                "data": "price",
                "render": $.fn.dataTable.render.number('.', ',', 2, 'R$ '),
                "width": "10%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Product/Upsert/${data}" class="btn-sm btn-primary mx-2" title="Edit" style="text-decoration:none;">
                                    <i class="bi bi-pencil-square"></i>
                                </a>
                                <a onclick="Delete(${data})" class="btn-sm btn-danger mx-2" title="Delete" style="text-decoration:none; cursor:pointer">
                                    <i class="bi bi-trash-fill"></i>
                                </a>
                            </div>`;
                },
                "width": "10%"
            }
        ],
        "language": {
            "emptyTable": "No data found."
        },
        "width": "100%"
    });
}

function Delete(id) {
    let apiUrl = `/api/product/${id}`;

    Swal.fire({
        title: "Are you sure you want to delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: apiUrl,
                type: "DELETE",
                success: function (data) {
                    toastr.options.closeButton = true;
                    toastr.options.progressBar = true;

                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}

$(document).ready(function () {
    loadDataTable();
});
