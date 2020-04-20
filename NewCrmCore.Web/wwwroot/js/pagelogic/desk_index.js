
HROS.request.get('/desk/getdesktopinfo', {}, (responseText) => {

}, HROS.cookie.get('Token'))



document.body.onselectstart = document.body.ondrag = function () {
    return false;
};

HROS.CONFIG.dockPos = '@userConfig.DockPosition',
    HROS.CONFIG.appXY = '@userConfig.AppXy',
    HROS.CONFIG.appSize = parseInt('@userConfig.AppSize'),
    HROS.CONFIG.desk = parseInt('@userConfig.DefaultDeskNumber'),
    HROS.CONFIG.appHorizontalSpacing = '@userConfig.AppHorizontalSpacing',
    HROS.CONFIG.appVerticalSpacing = '@userConfig.AppVerticalSpacing',
    HROS.CONFIG.deskCount = parseInt('@desks'),
    HROS.CONFIG.lockPwdAndLoginPwd = '@(user.Password == user.LockScreenPassword ? 1 : 0)'
top.HROS.base.init();
$(".loading").hide();
$("#desktop").show();