/***
 * <input type="hidden" id="info" value="@LogLevel.Info"> 
<input type="hidden" id="warn" value="@LogLevel.Warning"> 
<input type="hidden" id="debug" value="@LogLevel.Debug"> 
<input type="hidden" id="error" value="@LogLevel.Error"> 
<input type="hidden" id="exc" value="@LogLevel.Exception">  */

//加载列表
getPageList(0);

//搜索
$('a[menu=search]').click(() => getPageList(0));

function getPageList(current_page) {
    HROS.request.get('/logger/getlogs/', {
        userName: $('#userName').val(),
        logLevel: $('#logLevel').find("option:selected").attr('value'),
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
                items_per_page: 7,
                num_display_entries: 5,
                num_edge_entries: 2,
                getPageList: getPageList
            });
        } else {
            NewCrm.fail(responseText.Message);
        }
    });
}

Handlebars.registerHelper('cutLongString', (str) => {
    if (str.length >= 30) {
        return str.substring(0, 30);
    }
});

Handlebars.registerHelper('parseLoggerLevel', (str) => {
    if (str === $('#info').val()) {
        return '信息';
    } else if (str === $('#warn').val()) {
        return '警告';
    } else if (str === $('#debug').val()) {
        return '调试';
    } else if (str === $('#error').val()) {
        return '业务错误';
    } else if (str === $('#exc').val()) {
        return '代码异常';
    }
});