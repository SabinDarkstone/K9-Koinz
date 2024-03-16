// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $("#txtMerchant").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/api/Autocomplete/GetAutocompleteMerchants",
                data: { text: request.term },
                type: "GET",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(JSON.stringify(response));
                },
                failure: function (response) {
                    alert(JSON.stringify(response));
                }
            });
        },
        select: function (e, i) {
            $("#hfMerchant").val(i.item.val);
            $.ajax({
                url: "/api/Autocomplete/GetSuggestedCategory",
                data: { merchantId: $("#hfMerchant").val() },
                type: "GET",
                success: function (data) {
                    if (data != null) {
                        $("#hfCategory").val(data.id);
                        $("#txtCategory").val(data.name);
                    }
                },
                error: function (data) {
                    console.log(data);
                },
                failure: function (data) {
                    console.log(data);
                }
            })
        },
        minLength: 1
    });

    $("#txtCategory").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/api/Autocomplete/GetAutocompleteCategories",
                data: { text: request.term },
                type: "GET",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(JSON.stringify(response));
                },
                failure: function (response) {
                    alert(JSON.stringify(response));
                }
            });
        },
        select: function (e, i) {
            $("#hfCategory").val(i.item.val);
        },
        minLength: 1
    });
});