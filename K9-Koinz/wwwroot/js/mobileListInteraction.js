$(".transaction-item").click(function () {
    window.location = "/Transactions/Details?id=" + $(this).data("id");
    return false;
});

$(".bill-item").click(function () {
    window.location = "/Bills/Details?id=" + $(this).data("id");
    return false;
});