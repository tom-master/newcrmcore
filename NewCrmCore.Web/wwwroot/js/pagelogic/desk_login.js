let f = $('#form').Validform({
    btnSubmit: '#btn-submit',
    postonce: true,
    showAllError: true,
    tiptype: (msg, o) => {
        if (!o.obj.is('form')) {
            let B = o.obj.parents('form-control');
            let T = B.children('.help-inline');
            if (o.type === 2) {
                T.text('');
            } else {
                alert(msg);
            }
        }
    },
    ajaxPost: true,
    postonceTip: () => {
        NewCrm.fail(HROS.CONFIG.postOnce);
    },
    beforeSubmit: (curform) => {
        NewCrm.loading(HROS.CONFIG.loadingPrompt);
    },
    callback: (responseText) => {
        let response = responseText;
        if (response.IsSuccess) {
            location.href = '/desk/index';
        } else {
            NewCrm.fail(response.Message);
        }
    }
});

$(document).keyup((e)=>{
    let key = e.which;
    if(key==13){
        $('#btn-submit').click();
    }
})
