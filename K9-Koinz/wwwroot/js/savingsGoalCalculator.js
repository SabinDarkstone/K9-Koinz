let startDate;
let endDate;
let startAmount;
let targetAmount;
let contributions;

$(function () {
    contributions = parseFloat($("#Record_SavedAmount").val()) || 0;
    targetAmount = parseFloat($("#Record_TargetAmount").val());
    startAmount = parseFloat($("#Record_StartingAmount").val());
    endDate = Date.parse($("#Record_TargetDate").val());
    startDate = Date.parse($("#Record_StartDate").val());

    recalculate();
});

$(".recalculate").on("keyup change touchend", function () {
    const fieldId = $(this).attr("id");
    console.log(fieldId);

    if (fieldId.includes("TargetAmount")) {
        targetAmount = parseFloat($(this).val());
    }

    if (fieldId.includes("StartDate")) {
        startDate = Date.parse($(this).val());
    }

    if (fieldId.includes("TargetDate")) {
        endDate = Date.parse($(this).val());
    }

    if (fieldId.includes("StartingAmount")) {
        startAmount = parseFloat($(this).val());
    }

    recalculate();
});

function recalculate() {
    //console.log("startDate", startDate);
    //console.log("endDate", endDate);
    //console.log("startAmount", startAmount);
    //console.log("targetAmount", targetAmount);

    let remainingAmount = targetAmount - startAmount - contributions;
    console.log("remainingAmount", remainingAmount);
    let remainingMonths = monthDiff(startDate, endDate);
    console.log("remainingMonths", remainingMonths);

    let monthlyAmount = remainingAmount / remainingMonths;
    console.log(monthlyAmount);

    if (isNaN(monthlyAmount)) {
        $("#calculatedAmount").val("Target Will Display Here")
    } else if (monthlyAmount < 0) {
        $("#calculatedAmount").val("Invalid Settings");
    } else if (isFinite(monthlyAmount)) {
        $("#calculatedAmount").val("$" + monthlyAmount.toFixed(2));
    } else {
        $("#calculatedAmount").val("Unknown Error");
    }
}

function monthDiff(d1, d2) {
    d1 = new Date(d1);
    d2 = new Date(d2);

    d2.setUTCDate(d1.getUTCDate());

    var months;
    months = (d2.getFullYear() - d1.getFullYear()) * 12;
    months -= d1.getMonth();
    months += d2.getMonth();
    return months <= 0 ? 0 : months;
}