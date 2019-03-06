//加载列表
getPageList(0);

NewCrm.AppManager.Index = {
    wait: 0,
    pass: 0,
    url: ''
};

//删除，推荐
$('.list-con').on('click', '.do-del', function () {
    let appid = $(this).attr('appid');
    let appname = $(this).parents('tr').children('td:nth-child(2)').text();

    $.dialog({
        id: 'del',
        content: '确定要删除 “' + appname + '” 该应用么？',
        ok: () => {
            HROS.request.post('/appmanager/remove', { appId: appid }, (responseText) => {
                if (responseText.IsSuccess) {

                    $('#pagination').trigger('currentPage');
                } else {
                    NewCrm.msgbox.fail(responseText.Message);
                }
            });
        },
        cancel: true
    });
}).on('click', '.do-recommend', function () {
    let appid = $(this).attr('appid');
    HROS.request.post('/appmanager/recommend', { appId: appid }, (responseText) => {
        if (responseText.IsSuccess) {
            $('#pagination').trigger('currentPage');
        } else {
            NewCrm.msgbox.fail(responseText.Message);
        }
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

    HROS.request.get('/appmanager/getapps',
        {
            searchText: $('#appName').val(),
            appTypeId: $('#appType').find("option:selected").attr('value'),
            appStyleId: $('#appStyle').find("option:selected").attr('value'),
            appState: appStateResult,
            pageIndex: current_page === 0 ? 1 : current_page + 1,
            pageSize: 7
        }, (responseText) => {
            if (responseText.IsSuccess) {
                let tableTemplate = Handlebars.compile($("#table-template").html());
                $('.list-con').html(tableTemplate(responseText));
                $('.list-count').text(responseText.TotalCount);
                $('#pagination_setting').attr('count', parseInt(responseText.TotalCount));
                initPagination({
                    current_page: current_page,
                    items_per_page: 7,
                    num_display_entries: 9,
                    num_edge_entries: 2,
                    getPageList: getPageList
                });
            } else {
                NewCrm.msgbox.fail(responseText.Message);
            }
        });

    Handlebars.registerHelper("isAudit", function (v1, options) {
        if (v1 === NewCrm.AppManager.pass) {
            return options.fn(this);
        } else {
            return options.inverse(this);
        }
    });

    Handlebars.registerHelper('convertstyle', (appStyle) => {
        if (appStyle === 1) {
            return '应用';
        } else {
            return '挂件';
        }
    });

    Handlebars.registerHelper('appendUrl', (v1, v2) => {
        if (v1) {
            return NewCrm.AppManager.url + v2;
        }
        return v2;
    });

    Handlebars.registerHelper("compare", function (v1, v2, options) {
        if (v1 === v2 || v1 === NewCrm.AppManager.wait) {
            return options.fn(this);
        } else {
            return options.inverse(this);
        }
    });

    Handlebars.registerHelper("remove", function (v1, options) {
        if (v1) {
            return options.fn(this);
        } else {
            return options.inverse(this);
        }
    });
}