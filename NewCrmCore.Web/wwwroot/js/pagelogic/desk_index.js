NewCrm.Desk.Index = {
    dockPosition: '',
    appXy: '',
    appSize: 0,
    defaultDeskNumber: 0,
    appHorizontalSpacing: '',
    AppVerticalSpacing: '',
    desks: 0,
    lockPwdAndLoginPwd: 0
};

document.body.onselectstart = document.body.ondrag = function () {
    return false;
};
HROS.CONFIG.dockPos = NewCrm.Desk.Index.dockPosition;
HROS.CONFIG.appXY = NewCrm.Desk.Index.appXy;
HROS.CONFIG.appSize = NewCrm.Desk.Index.appSize;
HROS.CONFIG.desk = NewCrm.Desk.Index.defaultDeskNumber;
HROS.CONFIG.appHorizontalSpacing = NewCrm.Desk.Index.appHorizontalSpacing;
HROS.CONFIG.appVerticalSpacing = NewCrm.Desk.Index.appVerticalSpacing;
HROS.CONFIG.deskCount = NewCrm.Desk.Index.desks;
HROS.CONFIG.lockPwdAndLoginPwd = NewCrm.Desk.Index.lockPwdAndLoginPwd;
HROS.base.init();

$(".loading").hide();
$("#desktop").show();