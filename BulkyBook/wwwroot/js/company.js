var dataTable;

function loadDataTable() {
    dataTable = $("#companies").DataTable({
        "ajax": {
            "url": "/api/companies"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "25%" },
            { "data": "streetAddress", "width": "20%" },
            { "data": "city", "width": "15%" },
            { "data": "state", "width": "10%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Admin/Company/Upsert/${data}" class="btn-sm btn-primary mx-2" title="Edit" style="text-decoration:none;">
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
    let apiUrl = `/api/company/${id}`;

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
