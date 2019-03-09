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
        let response = responseText;
        if (response.IsSuccess) {
            if ($('input[name="id"]').val()) {
                if ($('#pwd').val()) {
                    NewCrm.msgbox.success('您修改了登陆密码，请重新登陆');
                    setTimeout(() => top.location.reload(), 1000);
                } else {
                    NewCrm.msgbox.success('修改成功!');
                    setTimeout(() => {
                        window.parent.closeDetailIframe(() => window.parent.$('#pagination').trigger('currentPage'));
                    }, 1000);
                }
            } else {
                $.dialog({
                    id: 'ajaxedit',
                    content: '添加成功，是否继续添加?',
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
            NewCrm.msgbox.fail(response.Message);
        }
    }
});

$('input[name="val_type"]').change(function () {
    if (parseInt($(this).val()) === 2) {
        $('.input-label-permission').slideDown();
    } else {
        $('#roleIds').val('');
        $('.input-label-permission').slideUp();
    }
});

$('input[name="val_permission_id"]').on('click', () => {
    $('#roleIds').val('');
    let ids = '';
    $.each($('input[name="val_permission_id"]').filter(':checked'), (k, v) => {
        ids += $(v).attr('id') + ',';
    });
    $('#roleIds').val(ids);
    console.log($('#roleIds').val());
})