function changeAmountColor(field) {
    let amount = parseFloat(field.val());
    if (amount > 0) {
        field.addClass("txt-positive");
        field.removeClass("txt-negative");
    } else if (amount < 0) {
        field.addClass("txt-negative");
        field.removeClass("txt-positive");
    } else {
        field.removeClass("txt-negative");
        field.removeClass("txt-positive");
    }
}