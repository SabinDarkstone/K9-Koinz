$(".txtMerchant").each((idx, item) => {
    let dataIndex = item.dataset.index;
    let txtId = "#txtMerchant-" + dataIndex;
    let hfId = "#hfMerchant-" + dataIndex;

    $(txtId).autocomplete({
        source: (request, response) => autocompleteMerchants(request, response),
        select: function (e, i) {
            $(hfId).val(i.item.id);
        },
        minLength: 1
    });
})

$(".txtCategory").each((idx, item) => {
    let dataIndex = item.dataset.index;
    let txtId = "#txtCategory-" + dataIndex;
    let hfId = "#hfCategory-" + dataIndex;

    $(txtId).autocomplete({
        source: (request, response) => autocompleteCategories(request, response),
        select: function (e, i) {
            $(hfId).val(i.item.id);
        },
        minLength: 1
    });
})