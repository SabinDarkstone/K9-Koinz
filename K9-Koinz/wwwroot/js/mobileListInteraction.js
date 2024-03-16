$(".transaction-item").click(function () {
    window.location = "/Transactions/Details?id=" + $(this).data("id");
    return false;
});