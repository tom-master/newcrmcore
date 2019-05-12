//加载列表
getPageList(0);
openDetailIframe2 = (url) => {
    $('#detailIframe iframe').attr('src', url).on('load', () => {
        $('#detailIframe').animate({
            'left': '60px',
            'opacity': 'show'
        }, 500);
    });
};

closeDetailIframe2 = (callback) => {
    $('#detailIframe').animate({
        'left': 0,
        'opacity': 'hide'
    }, 500, () => {
        $('#detailIframe').css('left', '140px');
        callback && callback();
    });
};

$('.sub-nav ul li').click(function () {
    closeDetailIframe2();
    $('.sub-nav ul li').removeClass('active');
    $(this).addClass('active');
    $('#appType').val($(this).attr('value'));
    $('.app-list-box .title li').removeClass('active').eq(0).addClass('active');
    $('#order').val(1);
    getPageList(0);
});
$('.app-list-box .title li').click(function () {
    $('.app-list-box .title li').removeClass('focus');
    $(this).addClass('focus');
    $('#order').val($(this).attr('value'));
    getPageList(0);
});
//搜索
$('#searchText').on('keydown', (e) => {
    if (e.keyCode === '13') {
        $('#searchTextDo').click();
    }
});
$('#searchTextDo').click(() => {
    $('.app-list-box .title li').removeClass('focus').eq(0).addClass('focus');
    $('.sub-nav ul li').removeClass('active').eq(0).addClass('active');
    $('#appType').val(0);
    $('#order').val(1);
    getPageList(0);
});
$('#searchTextRemove').click(() => {
    $('#searchText').val('');
    getPageList(0);
});
//添加，删除，打开应用
$('.app-list').on('click', '.btn-add-s', function () {
    if (top.HROS.base.checkLogin()) {
        $(this).removeClass().addClass('btn-loading-s');
        top.HROS.app.add($(this).attr('real_app_id'), () => {
            $('#pagination').trigger('currentPage');
            top.HROS.app.get();
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
}).on('click', '.btn-remove-s', function () {
    if (top.HROS.base.checkLogin()) {
        $(this).removeClass().addClass('btn-loading-s');
        let realappid = $(this).attr('real_app_id'), type = $(this).attr('app_type');
        top.HROS.app.remove($(this).attr('app_id'), () => {
            $('#pagination').trigger('currentPage');
            top.HROS.widget.removeCookie(realappid, type);
            top.HROS.app.get();
        });
    } else {
        top.HROS.base.login();
    }
}).on('click', '.btn-run-s', function () {
    if ($(this).attr('app_type') === 'app') {
        top.HROS.window.create($(this).attr('real_app_id'), $(this).attr('app_type'));
    } else {
        top.HROS.widget.create($(this).attr('real_app_id'), $(this).attr('app_type'));
    }
});
$('.commend-day').on('click', '.btn-add', function () {
    if (top.HROS.base.checkLogin()) {
        let appid = $(this).attr('real_app_id');
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
}).on('click', '.btn-run', function () {
    if ($(this).attr('app_type') === 'app') {
        top.HROS.window.create($(this).attr('real_app_id'));
    } else {
        top.HROS.widget.create($(this).attr('real_app_id'));
    }
});
function getPageList(current_page) {
    debugger
    let appTypeId = $('#appType').val() === '' ? 0 : parseInt($('#appType').val());
    let orderId = $('#order').val() === '' ? 1 : parseInt($('#order').val());
    let outSideSearchText = getQueryString('searchText');
    HROS.request.get('/appmarket/getapps', {
        appTypeId: appTypeId,
        orderId: orderId,
        searchText: outSideSearchText ? outSideSearchText : $('#searchText').val(),
        pageIndex: current_page === 0 ? 1 : current_page + 1,
        pageSize: 5
    }, (responseText) => {
        if (responseText.IsSuccess) {
            let liTemplate = Handlebars.compile($("#li-template").html());
            $('.app-list').html(liTemplate(responseText));
            $('#pagination_setting').attr('count', parseInt(responseText.TotalCount));
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

    Handlebars.registerHelper('scoreWidth', (score) => score * 20);

    Handlebars.registerHelper('parseScore', (score) => parseFloat(score).toFixed(1));

    Handlebars.registerHelper('IsInstall', function (isInstall, options) {
        if (isInstall) {
            return options.fn(this);
        } else {
            return options.inverse(this);
        }
    });
}