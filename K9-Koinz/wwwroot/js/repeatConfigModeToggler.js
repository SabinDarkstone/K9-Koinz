$(function () {
    $('#grpIntervalGap').hide();

    $('#modeSelector').change(function () {
        if (this.value == 1) {
            $('#grpIntervalGap').show();
        } else {
            $('#grpIntervalGap').hide();
        }
    })
});