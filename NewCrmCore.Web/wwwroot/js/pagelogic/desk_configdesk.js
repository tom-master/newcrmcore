$('input[name="desk"]').change(() => {
    let desk = $('input[name="desk"]:checked').val();
    top.HROS.deskTop.updateDefaultDesk(desk);
});
$('input[name="wallpapersource"]').change(() => {
    let val = $('input[name="wallpapersource"]:checked').val();
    top.HROS.deskTop.modifyWallpaperSource(val);
});

$('input[name="appxy"]').change(() => {
    let xy = $('input[name="appxy"]:checked').val();
    top.HROS.app.updateXY(xy);
});

//更新应用图标大小
$('.appsize-minus, .appsize-plus').click(function () {
    let size = parseInt($('input[name="appsize"]').val());
    if ($(this).hasClass('appsize-minus')) {
        size = size - 1;
    } else {
        size = size + 1;
    }
    updateSize(size);
});

$('input[name="appsize"]').keyup(() => {
    let size = parseInt($('input[name="appsize"]').val());
    updateSize(size);
});

let updateSize = (size) => {
    if (size < 32) {
        size = 32;
    } else if (size > 64) {
        size = 64;
    }
    $('input[name="appsize"]').val(size);
    top.HROS.app.updateSize(size);
};

//更新图标垂直间距
$('.appverticalspacing-minus, .appverticalspacing-plus').click(function () {
    let vertical = parseInt($('input[name="appverticalspacing"]').val());
    if ($(this).hasClass('appverticalspacing-minus')) {
        vertical = vertical - 1;
    } else {
        vertical = vertical + 1;
    }
    updateVertical(vertical);
});

$('input[name="appverticalspacing"]').keyup(() => {
    let vertical = parseInt($('input[name="appverticalspacing"]').val());
    updateVertical(vertical);
});

$('.apphorizontalspacing-minus, .apphorizontalspacing-plus').click(function () {
    let horizontal = parseInt($('input[name="apphorizontalspacing"]').val());
    if ($(this).hasClass('apphorizontalspacing-minus')) {
        horizontal = horizontal - 1;
    } else {
        horizontal = horizontal + 1;
    }
    updateHorizontal(horizontal);
});

$('input[name="apphorizontalspacing"]').keyup(() => {
    let horizontal = parseInt($('input[name="apphorizontalspacing"]').val());
    updateHorizontal(horizontal);
});

//更新应用码头位置
$('input[name="dockpos"]').change(() => {
    let pos = $('input[name="dockpos"]:checked').val();
    $('.set_view').removeClass('set_view_top set_view_left set_view_right set_view_none');
    $('.set_view').addClass('set_view_' + pos);
    top.HROS.dock.updatePos(pos);
});

//更新图标水平间距
function updateHorizontal(horizontal) {
    if (horizontal < 0) {
        horizontal = 0;
    } else if (horizontal > 100) {
        horizontal = 100;
    }
    $('input[name="apphorizontalspacing"]').val(horizontal);
    top.HROS.app.updateHorizontal(horizontal);
}

function updateVertical(vertical) {
    if (vertical < 0) {
        vertical = 0;
    } else if (vertical > 100) {
        vertical = 100;
    }
    $('input[name="appverticalspacing"]').val(vertical);
    top.HROS.app.updateVertical(vertical);
}