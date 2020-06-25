//添加应用
$('.btn-add').click(function () {
    if (top.HROS.base.checkLogin()) {
        let appid = $(this).attr('app_id');
        top.HROS.app.add(appid, () => {
            top.HROS.app.get();
            location.reload();
        });
    } else {
        window.parent.$.dialog({
            title: '温馨提示',
            icon: 'warning',
            content: '您尚未登录，赶快登录去添加您喜爱的应用吧！',
            ok: () => {
                top.HROS.base.login();
            }
        });
    }
});
//打开应用
$('.btn-run').click(function () {
    if ($(this).attr('app_type') === 'app') {
        top.HROS.window.create($(this).attr('app_id'));
    } else {
        top.HROS.widget.create($(this).attr('app_id'));
    }
});
//评分
$('.grade-box ul li').click(function () {
    let num = $(this).attr('num');
    if (!isNaN(num) && /^[1-5]$/.test(num)) {
        if (top.HROS.base.checkLogin()) {
            HROS.request.post('/appmarket/modifystar', { AppId: '@Model.Id', StarCount: num }, (responseText) => {
                if (responseText.IsSuccess) {
                    NewCrm.success("打分成功！");
                    setTimeout(() => { location.reload(); }, 2000);
                } else {
                    NewCrm.fail(responseText.Message);
                }
            });
        } else {
            top.HROS.base.login();
        }
    }
}); 