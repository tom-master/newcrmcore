//加载列表
getPageList(0);

//删除
$('.list-con').on('click', '.do-del', function () {
    let roleId = $(this).attr('data-roleid');
    let name = $(this).parent().prev().text();
    $.dialog({
        id: 'ajaxedit',
        content: '确定要删除 "' + name + '" 该角色？',
        ok: () => {
            HROS.request.post('/security/removerole', { roleId: roleId }, (responseText) => {
                if (responseText.IsSuccess) {
                    setTimeout(() => {
                        $('#pagination').trigger('currentPage');
                    }, 1000);
                    NewCrm.success('删除成功');
                } else {
                    NewCrm.fail(responseText.Message);
                }
            });
        },
        cancel: true
    });
});

//搜索
$('a[menu=search]').click(() => getPageList(0));

function getPageList(current_page) {
    HROS.request.get('/security/getroles', {
        roleName: $('#roleName').val(),
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
            NewCrm.fail(responseText.Message);
        }
    });
}