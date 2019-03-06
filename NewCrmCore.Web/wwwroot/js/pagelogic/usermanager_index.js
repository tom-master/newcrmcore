//加载列表
getPageList(0);
//删除
$('.list-con').on('click', '.do-del', function () {
    let memberid = $(this).attr('memberid');
    let name = $(this).parent().prev().text();
    art.dialog({
        id: 'edit',
        content: '确定要删除 “' + name + '” 该账户么？',
        ok: () => {
            HROS.request.post('/usermanager/removeuser', { userId: memberid }, (responseText) => {
                if (responseText.IsSuccess) {
                    $('#pagination').trigger('currentPage');
                } else {
                    NewCrm.msgbox.fail(responseText.Message);
                }
            });
        },
        cancel: true
    });
});

$('.list-con').on('click', '.do-disable', function () {
    let memberid = $(this).attr('memberid');
    let name = $(this).parent().prev().text();
    let isDisable = $(this).attr('isDisable');
    art.dialog({
        id: 'edit',
        content: isDisable ? `确定要启用"${name}"该账户么?` : `确定要禁用"${name}"该账户么?`,
        ok: () => {
            let url = '';
            if (isDisable) {
                url = '/usermanager/disable';
            } else {
                url = '/usermanager/enable';
            }
            HROS.request.post(url, { userId: parseInt(memberid) }, (responseText) => {
                if (responseText.IsSuccess) {
                    $('#pagination').trigger('currentPage');
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
    HROS.request.get('/usermanager/users', {
        userName: $('#userName').val(),
        userType: $('#userType').val(),
        pageIndex: current_page === 0 ? 1 : current_page + 1,
        pageSize: 5
    }, (responseText) => {
        if (responseText.IsSuccess) {
            let tableTemplate = Handlebars.compile($("#table-template").html());
            $('.list-con').html(tableTemplate(responseText));
            $('#pagination_setting').attr('count', responseText.TotalCount);
            $('.list-count').text(responseText.TotalCount);
            initPagination({
                current_page: current_page,
                items_per_page: 5,
                num_display_entries: 3,
                num_edge_entries: 1,
                getPageList: getPageList
            });
        } else {
            NewCrm.msgbox.fail(responseText.Message);
        }
    });
}

Handlebars.registerHelper('convertUserType', (isAdmin) => {
    if (isAdmin) {
        return '管理员账户';
    }
    return '普通账户';
});