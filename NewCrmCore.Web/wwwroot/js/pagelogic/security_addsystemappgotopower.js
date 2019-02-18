if ($.dialog.data('appsid') !== '') {
    $('#value_1').val($.dialog.data('appsid'));
    let appsid = $.dialog.data('appsid').split(',');
    $('.app').each(() => {
        for (let i = 0; i < appsid.length; i++) {
            if (appsid[i] === $(this).attr('appid')) {
                $(this).addClass('act');
                break;
            }
        }
    });
}
$('.app').click(() => {
    if ($(this).hasClass('act')) {
        let appsid = $('#value_1').val().split(',');
        let newappsid = [];
        for (let i = 0, j = 0; i < appsid.length; i++) {
            if (appsid[i] !== $(this).attr('appid')) {
                newappsid[j] = appsid[i];
                j++;
            }
        }
        $('#value_1').val(newappsid.join(','));
        $(this).removeClass('act');
    } else {
        if ($('#value_1').val() !== '') {
            let appsid = $('#value_1').val().split(',');
            appsid[appsid.length] = $(this).attr('appid');
            $('#value_1').val(appsid.join(','));
        } else {
            $('#value_1').val($(this).attr('appid'));
        }
        $(this).addClass('act');
    }
    $.dialog.data('appsid', $('#value_1').val());
});