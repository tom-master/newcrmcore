
NewCrm.AppMarket.UserAppManageDetail = {
    app: 0,
    url: ''
};

$('html').css({ 'overflow-y': 'hidden' });
//加载列表
getPageList(0);
//删除
$('.list-con').on('click', '.do-del', function () {
    let appId = $(this).attr('appId');
    let appname = $(this).parents('tr').children('td:first-child').text();
    $.dialog({
        id: 'del',
        content: '确定要删除 “' + appname + '” 该应用么？',
        ok: () => {
            HROS.request.post('/appmarket/remove', { appId: appId }, (responseText) => {
                if (responseText.IsSuccess) {
                    NewCrm.msgbox.success('删除成功!');
                    setTimeout(() => {
                        $('#pagination').trigger('currentPage');
                    }, 1000);
                } else {
                    NewCrm.msgbox.fail(responseText.Message);
                }
            });
        },
        cancel: true
    });
});
//搜索
$('a[menu=search]').click(() => {
    getPageList(0);
});
function getPageList(current_page) {
    let $appStates = $('#appState').find("option:selected");
    let appStateResult = '';
    if (parseInt($appStates.attr('value')) !== 0) {
        appStateResult = $appStates.attr('data-type') + ',' + $appStates.attr('value');
    }

    HROS.request.get('/appmarket/getuserapps',
        {
            searchText: $('#appName').val(),
            appTypeId: $('#appType').find("option:selected").attr('value'),
            appStyleId: $('#appStyle').find("option:selected").attr('value'),
            appState: appStateResult,
            pageIndex: current_page === 0 ? 1 : current_page + 1,
            pageSize: 4
        }, (responseText) => {
            if (responseText.IsSuccess) {
                let tableTemplate = Handlebars.compile($("#table-template").html());
                $('.list-con').html(tableTemplate(responseText));
                $('.list-count').text(responseText.totalCount);
                $('#pagination_setting').attr('count', parseInt(responseText.TotalCount));
                initPagination({
                    current_page: current_page,
                    items_per_page: 4,
                    num_display_entries: 5,
                    num_edge_entries: 2,
                    getPageList: getPageList
                });
            } else {
                NewCrm.msgbox.fail(responseText.Message);
            }
        });

    Handlebars.registerHelper('convertstyle', (appStyle) => {
        if (appStyle === NewCrm.AppMarket.UserAppManageDetail.app) {
            return '应用';
        }
        return '挂件';
    });

    Handlebars.registerHelper('appendUrl', (v1, v2) => {
        if (v1) {
            return NewCrm.AppMarket.UserAppManageDetail.url + v2;
        }
        return v2;
    });
}