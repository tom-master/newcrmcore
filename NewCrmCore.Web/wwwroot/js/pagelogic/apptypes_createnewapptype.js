NewCrm.AppTypes = {
    CreateNewAppType:{
        appTypeId: 0
    }
};

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
            }
            else {
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
            if (NewCrm.AppTypes.CreateNewAppType.appTypeId) {
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
            }
            else {
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
        }
        else {
            NewCrm.msgbox.fail(responseText.Message);
        }
    }
});