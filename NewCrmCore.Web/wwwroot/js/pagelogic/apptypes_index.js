//加载列表
getPageList(0);
//删除
$('.list-con').on('click', '.do-del', function () {
    let categoryid = $(this).attr('categoryid');
    let name = $(this).parent().prev().text();
    $.dialog({
        id: 'ajaxedit',
        content: `删除${name}后将不可恢复!是否继续?`,
        ok: () => {
            HROS.request.post('/apptypes/remove', { appTypeId: categoryid }, (responseText) => {
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
$('a[menu=search]').click(() => getPageList(0));
function getPageList(current_page) {
    HROS.request.get('/apptypes/gettypes', {
        pageIndex: current_page === 0 ? 1 : current_page + 1,
        pageSize: 10,
        searchText: $('#searchText').val()
    }, (responseText) => {
        if (responseText.IsSuccess) {
            let template = Handlebars.compile($('#li-template').html());
            $('#pagination_setting').attr('count', parseInt(responseText.TotalCount));
            $('.list-count').text(parseInt(responseText.TotalCount));
            $('.list-con').html(template(responseText));
            initPagination({
                current_page: current_page,
                items_per_page: 6,
                num_display_entries: 3,
                num_edge_entries: 1,
                getPageList: getPageList
            });

        } else {
            NewCrm.msgbox.fail(responseText.Message);
        }
    });
}