$("#txtMerchant").autocomplete({
    source: (request, response) => autocompleteMerchants(request, response),
    select: function (e, i) {
        $("#hfMerchant").val(i.item.val);
        getSuggestedCategory($("#hfMerchant").val(), (data) => {
            $("#hfCategory").val(data.id);
            $("#txtCategory").val(data.name);
        });
    },
    minLength: 1
});

$("#txtCategory").autocomplete({
    source: (request, response) => autocompleteCategories(request, response),
    select: function (e, i) {
        $("#hfCategory").val(i.item.val);
    },
    minLength: 1
});

function getSuggestedCategory(merchantId, callback) {
    $.ajax({
        url: "/api/Autocomplete/GetSuggestedCategory",
        data: { merchantId: merchantId },
        type: "GET",
        success: function (data) {
            if (data != null) {
                callback(data);
            }
        },
        error: function (data) {
            console.log(data);
        },
        failure: function (data) {
            console.log(data);
        }
    });
}

function autocompleteMerchants(request, response) {
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
}

function autocompleteCategories(request, response) {
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
}