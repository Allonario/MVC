$(document).ready(function () {
    loadDataTable()
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        ajax: { url: '/admin/product/getall' },
        columns: [
            { data: 'title' },
            { data: 'author' },
            { data: 'isbn' },
            { data: 'category.name' },
            { data: 'price' },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/admin/product/upsert?id=${data}" class="btn btn-info mx-2" ><i class="bi bi-pencil"></i> Edit</a >
                        <a onClick=Delete("/admin/product/delete/${data}") class="btn btn-warning mx-2" ><i class="bi bi-pencil"></i> Delete</a >
                        </div >`
                }
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}