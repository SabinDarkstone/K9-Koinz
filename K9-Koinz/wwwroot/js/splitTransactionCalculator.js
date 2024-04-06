function calculateTotal() {
    let runningTotal = 0;
    $(".txtAmount").each(function () {
        let amount = parseFloat($(this).val()) || 0;
        runningTotal += amount;
    });
    //console.log("running total", runningTotal);
    $("#txtTotalAmount").val(runningTotal.toFixed(2));
    return runningTotal;
}

function checkDisableSaveButton() {
    let runningTotal = calculateTotal();

    // Determine if the save button should be disabled
    if (runningTotal.toFixed(2) != parentTransaction.Amount.toFixed(2)) {
        //console.log('disabling button because', runningTotal.toFixed(2), parentTransaction.Amount.toFixed(2));
        $("#btnSave").prop("disabled", true);
    } else {
        $("#btnSave").prop("disabled", false);
    }
    $(".txtAmount").each(function (idx) {
        const hfCategoryValue = $("#hfCategory-" + idx).val();

        //console.log(idx, "hfcategoryvalue", hfCategoryValue, $(this).val() && $(this).val() != 0 && $(this).val() != "");

        if ($(this).val() && $(this).val() != 0 && $(this).val() != "") {
            if (!hfCategoryValue) {
                console.log('disabling button because', idx);
                $("#btnSave").prop("disabled", true);
            }
        }
    });
}

$(document).ready(() => {
    let initialPopulatedCount = 0;
    $(".txtAmount").each(function (idx) {
        console.log(idx, $(this).val());
        if ($(this).val() && parseInt($(this).val()) != 0) {
            initialPopulatedCount += 1;
        }
    });

    console.log("num populated", initialPopulatedCount);

    if (initialPopulatedCount == 0) {
        initialPopulatedCount = 1;
    }
    $(".txtAmount").each(function (idx) {
        if (idx >= initialPopulatedCount) {
            $('tr[data-index="' + idx + '"]').hide();
        }
    });

    checkDisableSaveButton();
});

$(".txtAmount").keyup(function () {
    let runningTotal = calculateTotal();

    // Determine if the save button should be disabled
    if (runningTotal.toFixed(2) != parentTransaction.Amount.toFixed(2)) {
        //console.log('disabling button because', runningTotal.toFixed(2), parentTransaction.Amount.toFixed(2));
        $("#btnSave").prop("disabled", true);
    } else {
        $("#btnSave").prop("disabled", false);
    }

    checkDisableSaveButton();
});

$("input").keyup(function () {
    checkDisableSaveButton();
})

$("input").blur(function () {
    checkDisableSaveButton();
})

$("input").change(function () {
    console.log($(this).val());
});

$(".btnAddRow").click(function() {
    const rowIndex = $(this).data("index");
    if (rowIndex + 1 > 6) {
        return;
    }

    $('tr[data-index="' + (rowIndex + 1) + '"]').show();
    let runningTotal = calculateTotal();

    if (parentTransaction.Amount - runningTotal != 0) {
        $("#txtAmount-" + (rowIndex + 1)).val((parentTransaction.Amount - runningTotal).toFixed(2));
    }
    calculateTotal();
});

$(".btnRemoveRow").click(function () {
    const rowIndex = $(this).data("index");
    if (rowIndex == 0) {
        return;
    }

    let currentRow = $('tr[data-index="' + rowIndex + '"]')
    currentRow.hide();

    currentRow.find("input").each(function () {
        //console.log("clearing value from " + $(this));
        $(this).val("");
    });

    calculateTotal();
    checkDisableSaveButton();
});