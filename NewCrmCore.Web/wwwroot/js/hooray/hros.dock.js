/*
**  应用码头
*/
HROS.dock = (function () {
    return {
        /*
		**	初始化
		*/
        init: function () {
            HROS.dock.getPos(function () {
                HROS.dock.setPos();
            });
            //绑定应用码头拖动事件
            HROS.dock.move();
            var dockShowtopFunc;
            $('#dock-container').on('mouseenter', function () {
                dockShowtopFunc = setTimeout(function () {
                    $('#dock-container').addClass('showtop');
                }, 300);
            }).on('mouseleave', function () {
                clearInterval(dockShowtopFunc);
                $(this).removeClass('showtop');
            });
            $('body').on('contextmenu', '#dock-container', function (e) {
                HROS.popupMenu.hide();
                HROS.folderView.hide();
                HROS.searchbar.hide();
                HROS.startmenu.hide();
                var popupmenu = HROS.popupMenu.dock();
                var l = ($(window).width() - e.clientX) < popupmenu.width() ? (e.clientX - popupmenu.width()) : e.clientX;
                var t = ($(window).height() - e.clientY) < popupmenu.height() ? (e.clientY - popupmenu.height()) : e.clientY;
                popupmenu.css({
                    left: l,
                    top: t
                }).show();
                return false;
            });
            //绑定应用码头上各个按钮的点击事件
            $('#dock-bar .dock-tool-setting').on('mousedown', function () {
                return false;
            }).on('click', function () {
                if (HROS.base.checkLogin()) {
                    HROS.window.createTemp({
                        appid: 'hoorayos-zmsz',
                        title: '桌面设置',
                        url: '/PlatformSetting/DeskTopSetting',
                        width: 750,
                        height: 450,
                        isflash: false
                    });
                } else {
                    HROS.base.login();
                }
            });
            $('#dock-bar .dock-tool-style').on('mousedown', function () {
                return false;
            }).on('click', function () {
                if (HROS.base.checkLogin()) {
                    HROS.window.createTemp({
                        appid: 'hoorayos-ztsz',
                        title: '主题设置',
                        url: '/PlatformSetting/SystemWallPaper',
                        width: 580,
                        height: 520,
                        isflash: false
                    });
                } else {
                    HROS.base.login();
                }
            });
            $('#dock-bar .dock-tool-appmanage').on('mousedown', function () {
                return false;
            }).on('click', function () {
             
                HROS.appmanage.set();
               
            });
            $('#dock-bar .dock-tool-search').on('mousedown', function () {
                return false;
            }).on('click', function (e) {
                e.stopPropagation();
                HROS.searchbar.get();
            });
            $('#dock-bar .pagination').on('mousedown', function () {
                return false;
            }).on('click', function () {
                HROS.dock.switchDesk($(this).attr('index'));
            });
            $('#dock-bar .dock-tool-start').on('mousedown', function () {
                return false;
            }).on('click', function () {
                HROS.startmenu.show();
                return false;
            });
        },
        setPos: function () {
            HROS.dock.switchDesk(HROS.CONFIG.desk);
            var desktop = $('#desk-' + HROS.CONFIG.desk), desktops = $('#desk .desktop-container');
            var deskW = desktop.css('width', '100%').width(), deskH = desktop.css('height', '100%').height();
            //清除dock位置样式
            $('#dock-container').removeClass('dock-top dock-left dock-right');
            $('#dock-bar').removeClass('top-bar left-bar right-bar').hide();
            if (HROS.CONFIG.dockPos === 'top') {
                $('#dock-bar').addClass('top-bar').children('#dock-container').addClass('dock-top');
                desktops.css({
                    'width': deskW,
                    'height': deskH - $('#task-bar').height() - $('#dock-bar').height(),
                    'left': deskW,
                    'top': $('#dock-bar').height()
                });
                desktop.css({
                    'left': 0
                });
                $('#dock-bar').show();
            } else if (HROS.CONFIG.dockPos === 'left') {
                $('#dock-bar').addClass('left-bar').children('#dock-container').addClass('dock-left');
                desktops.css({
                    'width': deskW - $('#dock-bar').width(),
                    'height': deskH - $('#task-bar').height(),
                    'left': deskW + $('#dock-bar').width(),
                    'top': 0
                });
                desktop.css({
                    'left': $('#dock-bar').width()
                });
                $('#dock-bar').show();
            } else if (HROS.CONFIG.dockPos === 'right') {
                $('#dock-bar').addClass('right-bar').children('#dock-container').addClass('dock-right');
                desktops.css({
                    'width': deskW - $('#dock-bar').width(),
                    'height': deskH - $('#task-bar').height(),
                    'left': deskW,
                    'top': 0
                });
                desktop.css({
                    'left': 0
                });
                $('#dock-bar').show();
            } else if (HROS.CONFIG.dockPos === 'none') {
                desktops.css({
                    'width': deskW,
                    'height': deskH - $('#task-bar').height(),
                    'left': deskW,
                    'top': 0
                });
                desktop.css({
                    'left': 0
                });
            }
            HROS.taskbar.resize();
            HROS.folderView.setPos();
        },
        updatePos: function (pos) {
            if (pos !== HROS.CONFIG.dockPos && typeof (pos) != 'undefined') {
                HROS.CONFIG.dockPos = pos;
                if (pos === 'none') {
                    HROS.app.dataAllDockToDesk(HROS.CONFIG.desk);
                }
                if (HROS.base.checkLogin()) {
                    $.post('/PlatformSetting/ChangeDockPosition', { pos: pos, deskNum: HROS.CONFIG.desk }, function (result) {
                        if (parseInt(result.data) === 1) {
                            ZENG.msgbox.show('应用码头位置更新成功', 4, 2000);
                            //更新码头位置
                            HROS.dock.setPos();
                            //更新桌面应用
                            HROS.app.set();
                        } else {
                            ZENG.msgbox.show('应用码头位置更新失败', 5, 2000);
                        }
                    });
                }
            }
        },
        move: function () {
            $('#dock-container').on('mousedown', function (e) {
                if (e.button === 0 || e.button === 1) {
                    var lay = HROS.maskBox.dock(), location;
                    $(document).on('mousemove', function (e) {
                        lay.show();
                        if (e.clientY < lay.height() * 0.2) {
                            location = 'top';
                        } else if (e.clientX < lay.width() * 0.5) {
                            location = 'left';
                        } else {
                            location = 'right';
                        }
                        $('.dock_drap_effect').removeClass('hover');
                        $('.dock_drap_effect_' + location).addClass('hover');
                    }).on('mouseup', function () {
                        $(document).off('mousemove').off('mouseup');
                        lay.hide();
                        HROS.dock.updatePos(location);
                    });
                }
            });
        },
        /*
		**  切换桌面
		*/
        switchDesk: function (deskNumber) {
            //验证传入的桌面号是否为1-5的正整数
            var r = /^\+?[1-5]*$/;
            deskNumber = r.test(deskNumber) ? deskNumber : 1;
            var pagination = $('#dock-bar .dock-pagination'),
                currindex = HROS.CONFIG.desk,
                switchindex = deskNumber,
                currleft = $('#desk-' + currindex).offset().left,
                switchleft = $('#desk-' + switchindex).offset().left;
            if (currindex !== switchindex) {
                if (!$('#desk-' + switchindex).hasClass('animated') && !$('#desk-' + currindex).hasClass('animated')) {
                    $('#desk-' + currindex).addClass('animated').animate({
                        left: switchleft
                    }, 500, 'easeInOutCirc', function () {
                        $(this).removeClass('animated');
                    });
                    $('#desk-' + switchindex).addClass('animated').animate({
                        left: currleft
                    }, 500, 'easeInOutCirc', function () {
                        $(this).removeClass('animated');
                        pagination.removeClass('current-pagination-' + currindex).addClass('current-pagination-' + switchindex);
                        HROS.CONFIG.desk = switchindex;
                    });
                }
            } else {
                pagination.removeClass('current-pagination-' + currindex).addClass('current-pagination-' + switchindex);
            }
        },
        getPos: function (callback) {
            $.ajax({
                url: '/Index/GetDockPos',
                type: 'GET',
                data: { __: Date.now() },
                async: true,
                success: function (result) {
                    HROS.CONFIG.dockPos = result.data;
                    if (typeof (callback) === 'function') {
                        callback && callback();
                    }
                }
            });
        }
    };
})();