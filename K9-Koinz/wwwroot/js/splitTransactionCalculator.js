$(document).ready(() => {
    let initialPopulatedCount = 0;
    $(".txtAmount").each(function (idx) {
        console.log(idx, $(this).val());
        if ($(this).val() && parseInt($(this).val()) != 0) {
            initialPopulatedCount += 1;
        }
    });

    console.log("num populated", initialPopulatedCount);

    $(".txtAmount").each(function (idx) {
        if (idx >= initialPopulatedCount + 1) {
            $('tr[data-index="' + idx + '"]').hide();
        }
    });

    let runningTotal = 0;
    $(".txtAmount").each(function () {
        runningTotal += parseFloat($(this).val());
    });
    $("#txtTotalAmount").val(runningTotal.toFixed(2));
});

$(".txtAmount").change(function () {
    const totalBox = $("#txtTotalAmount");

    let runningTotal = 0;
    $(".txtAmount").each(function () {
        if ($(this).val().length > 0) {
            runningTotal += parseFloat($(this).val());
        }
    });
    $("#txtTotalAmount").val(runningTotal.toFixed(2));

    // Identify which indices have an amount
    let boxCount = 0;
    let lastIndexPopulated = -1;
    $(".txtAmount").each(function (idx) {
        boxCount++;
        if ($(this).val() != "" && $(this).val() != "0" && idx > lastIndexPopulated) {
            lastIndexPopulated = idx;
        }
    });

    // Hide any indices that are not populated, except for the next one
    if (lastIndexPopulated != boxCount) {
        let nextIndex = lastIndexPopulated + 1;
        $(".txtAmount").each(function (idx) {
            let currentRow = $('tr[data-index="' + idx + '"]');
            if (idx <= lastIndexPopulated || idx == nextIndex) {
                currentRow.show();
                if (idx == nextIndex) {
                    $(this).val((parentTransaction.Amount - runningTotal).toFixed(2));
                }
            } else {
                currentRow.hide();
            }

            if (!$(this).val()) {
                currentRow.hide();
            }
        });
    }

    runningTotal = 0;
    $(".txtAmount").each(function (idx) {
        console.log(idx, $(this).val());
        if ($(this).val().length > 0) {
            runningTotal += parseFloat($(this).val());
        }
    });
    $("#txtTotalAmount").val(runningTotal.toFixed(2));

    // Determine if the save button should be disabled
    if (runningTotal.toFixed(2) != parentTransaction.Amount) {
        $("#btnSave").prop("disabled", true);
    } else {
        $("#btnSave").prop("disabled", false);
    }

    $(".txtAmount").each(function (idx) {
        const hfMerchantValue = $("#hfMerchant-" + idx).val();
        const hfCategoryValue = $("#hfCategory-" + idx).val();

        if ($(this).val() && $(this).val() != 0 && $(this).val() != "") {
            if (!hfMerchantValue || !hfCategoryValue) {
                $("#btnSave").prop("disabled", true);
            }
        }
    });
});