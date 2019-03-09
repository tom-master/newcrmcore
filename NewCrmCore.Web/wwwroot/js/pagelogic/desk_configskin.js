loadAllSkin(() => changeSkin());

function loadAllSkin(callback) {
    HROS.request.get('/desk/getskins', {}, (responseText) => {
        if (responseText.IsSuccess) {
            let html = '';
            $.each(responseText.Model.result, (k, v) => {
                html += '<li skin="' + k + '"><img src="' + v.imgPath + '" data-css="' + v.cssPath + '"><div></div></li>';
            });
            $('#skinList').append(html);
            $('li[skin="' + responseText.Model.currentSkin + '"]').addClass('selected');
            if (typeof (callback) === 'function') {
                callback && callback();
            }
        } else {
            NewCrm.msgbox.fail(responseText.Message);
        }
    });
}

function changeSkin() {
    $('.skin li').on('click', function () {
        let skin = $(this).attr('skin');
        $('.skin li').removeClass('selected');
        $(this).addClass('selected');
        HROS.request.post('/desk/modifyskin', { skin: skin }, (responseText) => {
            if (responseText.IsSuccess) {
                top.NewCrm.msgbox.success("皮肤设置成功，如果没有更新请刷新页面");
                setTimeout(() => top.HROS.base.getSkin(skin), 2000);
            } else {
                NewCrm.msgbox.fail(responseText.Message);
            }
        });
    });
}