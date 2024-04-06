$("#btnAddMerchant").click((evt) => {
    const text = $("#txtMerchant").val();
    console.log(text);
    $.ajax({
        url: "/api/OnTheFlyCreate/AddMerchant",
        data: { text: text },
        type: "POST",
        success: function (data) {
            console.log(data);
            if (data == 'DUPLICATE') {
                setTimeout(() => {
                    $(evt.target).text("Add New");
                    $(evt.target).toggleClass("btn-success btn-warning");
                }, 2000);
                $(evt.target).text("Duplicate!");
                $(evt.target).toggleClass("btn-success btn-warning");
            } else if (data == 'ERROR') {
                setTimeout(() => {
                    $(evt.target).text("Add New");
                    $(evt.target).toggleClass("btn-success btn-danger");
                }, 2000);
                $(evt.target).text("Error!");
                $(evt.target).toggleClass("btn-success btn-danger");
            } else {
                $(evt.target).text("Success!");
                $("#hfMerchant").val(data);
            }
        }
    })
})