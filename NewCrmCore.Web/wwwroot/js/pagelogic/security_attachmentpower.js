let f = $('#form').Validform({
    btnSubmit: '#btn-submit',
    postonce: true,
    showAllError: true,
    tiptype: (msg, o) => {
        if (!o.obj.is('form')) {
            let B = o.obj.parents('.control-group');
            let T = B.children('.help-inline');
            if (o.type === 2) {
                B.removeClass('error');
                T.text('');
            } else {
                B.addClass('error');
                T.text(msg);
            }
        }
    },
    ajaxPost: true,
    postonceTip: () => {
        NewCrm.msgbox.fail(HROS.CONFIG.postOnce);
    },
    beforeSubmit: (curform) => {
        NewCrm.msgbox.loading(HROS.CONFIG.loadingPrompt);
    },
    callback: (responseText) => {
        if (responseText.IsSuccess) {
            if ($('input[name="val_roleId"]').val()) {
                $.dialog({
                    id: 'ajaxedit',
                    content: '修改成功，是否继续修改？',
                    okVal: '是',
                    ok: () => {
                        $.dialog.list['ajaxedit'].close();
                    },
                    cancel: () => {
                        window.parent.closeDetailIframe(() => {
                            window.parent.$('#pagination').trigger('currentPage');
                        });
                    }
                });
            } else {
                $.dialog({
                    id: 'ajaxedit',
                    content: '添加成功，是否继续添加？',
                    okVal: '是',
                    ok: () => {
                        location.reload();
                        return false;
                    },
                    cancel: () => {
                        window.parent.closeDetailIframe(() => {
                            window.parent.$('#pagination').trigger('currentPage');
                        });
                    }
                });
            }
        } else {
            NewCrm.msgbox.fail(responseText.Message);
        }
    }
});
//添加应用
$('a[menu=addapps]').click(() => {
    $.dialog.data('appsid', $('#val_apps_id').val());
    $.dialog.open('/security/addsystemappgotopower', {
        id: 'alert_addapps',
        title: '添加应用',
        resize: false,
        width: 360,
        height: 300,
        ok: () => {
            $('#val_apps_id').val($.dialog.data('appsid')).focusout();
            HROS.request.get('/security/getsystemapp', { appIds: $.dialog.data('appsid') }, (responseText) => {
                if (responseText.IsSuccess) {
                    let model = responseText.Model;
                    let msg = "";
                    for (let i = 0; i < model.length; i++) {
                        msg += '<div class="app" appid="' + model[i].Id + '">' +
                            '<img src="' + model[i].IconUrl + '" alt="' + model[i].Name + '" title="' + model[i].Name + '">' +
                            '<span class="del">删</span>' + ' </div > ';
                    }
                    $('.permissions_apps').html(msg);
                }
            });
        },
        cancel: true
    });
});
//删除应用
$('.permissions_apps').on('click', '.app .del', function () {
    let appid = $(this).parent().attr('appid');
    let appsid = $('#val_apps_id').val().split(',');
    let newappsid = [];
    for (let i = 0, j = 0; i < appsid.length; i++) {
        if (appsid[i] !== appid) {
            newappsid[j] = appsid[i];
            j++;
        }
    }
    $('#val_apps_id').val(newappsid.join(',')).focusout();
    $(this).parent().remove();
});