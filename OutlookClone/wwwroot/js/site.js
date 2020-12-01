// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function deleteMail(url, row, table_id) {
    $.ajax({
        url: url,
        type: 'DELETE',
        success: function(result) {
            console.log(result)
            let i = row.parentNode.parentNode.rowIndex;
            document.getElementById(table_id).deleteRow(i);
        }
    });
}