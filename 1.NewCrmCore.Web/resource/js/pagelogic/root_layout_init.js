//配置artDialog全局默认参数
((config) => {
    config['lock'] = true;
    config['fixed'] = true;
    config['resize'] = false;
    config['background'] = '#000';
    config['opacity'] = 0.5;
    config["skin"] = 'default';
})($.dialog.defaults);

//toolTip
$('[rel="tooltip"]').tooltip();
//表单提示
$("[datatype]").focusin(function () {
    $(this).parent().addClass('info').children('.infomsg').show().siblings('.help-inline').hide();
}).focusout(() => $(this).parent().removeClass('info').children('.infomsg').hide().siblings('.help-inline').show());

openDetailIframe = (url) => {
    $('#detailIframe iframe').attr('src', url).on('load', () => {
        $('body').css('overflow', 'hidden');
        $('#detailIframe').animate({ 'top': 0, 'opacity': 'show' }, 500);
    });
};
closeDetailIframe = (callback) => {
    $('body').css('overflow', 'auto');
    $('#detailIframe').animate({ 'top': '-100px', 'opacity': 'hide' }, 500, () => {
        if (callback) {
            callback && callback();
        }
    });
};

function initPagination(config) { 
    $('nav').pagination(parseInt($('#pagination_setting').attr('count')), {
        current_page: config.current_page,
        items_per_page: config.items_per_page,
        num_display_entries: config.num_display_entries,
        num_edge_entries: config.num_edge_entries,
        callback: config.getPageList,
        prev_text: '上一页',
        next_text: '下一页'
    });
}

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null){
        return unescape(r[2]);
    } 
    return null;
}