NewCrm.UserSetting.Index = {
    url: '',
    uploadUrl: '',
    id: 0
};

$('.title ul > li').on('click', function () {
    let $li = $(this);
    let value = $li.attr('data-value');
    $('.title ul > li').each((k, v) => {
        $(v).removeClass('focus');
    });
    let baseUrl = NewCrm.UserSetting.Index.url;
    if (value === 'baseinfo') {
        location.href = baseUrl + '?target=baseinfo';
    } else if (value === 'usersafe') {
        location.href = baseUrl + '?target=usersafe';
    }
});


let f1 = $('#form').Validform({
    btnSubmit: '#form-submit',
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
    ajaxPost: false,
    postonceTip: () => {
        NewCrm.msgbox.fail(HROS.CONFIG.postOnce);
    },
    beforeSubmit: (curform) => {
        NewCrm.msgbox.loading(HROS.CONFIG.loadingPrompt);
    },
    callback: (form) => {
        $.dialog({
            title: '温馨提示',
            icon: 'warning',
            content: '在更新登陆密码时，是否将锁屏密码一并修改为登陆密码？',
            button: [
                {
                    name: '否',
                    callback: () => {
                        $('#lockPwdIsEqLoginPwd').val('0');
                        form[0].submit();
                    },
                    focus: true
                },
                {
                    name: '是',
                    callback: () => {
                        $('#lockPwdIsEqLoginPwd').val('1');
                        form[0].submit();
                    }
                }
            ]
        });
        return false;
    }
});

let f2 = $('#form2').Validform({
    btnSubmit: '#form-submit2',
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
    ajaxPost: false,
    postonceTip: () => {
        NewCrm.msgbox.fail(HROS.CONFIG.postOnce);
    },
    beforeSubmit: (curform) => {
        NewCrm.msgbox.loading(HROS.CONFIG.loadingPrompt);
    },
    callback: (responseText) => {
        $.dialog({
            title: '温馨提示',
            icon: 'warning',
            content: '确认修改锁屏密码?',
            button: [
                {
                    name: '是',
                    callback: () => {
                        $('#lockPwdIsEqLoginPwd').val('0');
                        form[0].submit();
                    },
                    focus: true
                },
                {
                    name: '否',
                    callback: () => {
                        return false;
                    }
                }
            ]
        });
        return false;
    }
});

$('#hidden_frame').on('load', function () {
    let response = $(this).contents().find("body").text();
    if (response) {
        let responseText = JSON.parse($(this).contents().find("body").text());
        if (responseText.IsSuccess) {
            NewCrm.msgbox.success('您修改了登陆密码，2秒后跳转到登陆界面');
            setTimeout(function () {
                top.location.reload();
            }, 2000);
        } else {
            NewCrm.msgbox.fail('修改失败');
        }
    }
});

let upload = WebUploader.create({
    auto: true,
    swf: '~/js/webuploader-0.1.5/Uploader.swf',
    server: NewCrm.UserSetting.Index.uploadUrl,
    pick: {
        id: '#upload',
        multiple: false
    },
    accept: {
        title: 'Images',
        extensions: 'gif,jpg,jpeg,bmp,png',
        mimeTypes: 'image/*'
    },
    formData: {
        userId: NewCrm.UserSetting.Index.id,
        uploadtype: 'face'
    }
});

upload.on('beforeFileQueued', (file) => {
    if (file.size > 1000 * 1024) {
        NewCrm.msgbox.info('文件大于1000Kb');
        return false;
    }
});

upload.on('startUpload', () => NewCrm.msgbox.loading('账户头像上传中...'));

upload.on('uploadSuccess', (file, cb) => {
    if (cb[0].IsSuccess) {
        let model = {
            Title: cb[0].Title,
            Width: cb[0].Width,
            Height: cb[0].Height,
            Url: cb[0].Url,
            Source: 3,
            UserId: NewCrm.UserSetting.Index.id,
            Md5: cb[0].Md5,
            ShortUrl: ""
        };
        HROS.request.post('/usersetting/modifyface', { userFace: model.Url }, (responseText) => {
            if (responseText.IsSuccess) {
                NewCrm.msgbox.success('账户头像保存成功');
                top.HROS.startmenu.getAvatar();
            } else {
                NewCrm.msgbox.fail('账户头像保存失败');
            }
        });
    } else {
        NewCrm.msgbox.fail('账户头像保存失败');
    }
});