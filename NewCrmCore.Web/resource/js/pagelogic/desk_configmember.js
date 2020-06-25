

let uploader = WebUploader.create({
    auto: true,
    swf: '~/js/webuploader-0.1.5/Uploader.swf',
    server: $('#uploadUrl').val(),
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
        userId: $('#userId').val(),
        uploadtype: 'icon'
    }
});
uploader.on('beforeFileQueued', (file) => {
    if (file.size > 300 * 1024) {
        NewCrm.info('文件大于300Kb，请压缩后再上传');
        return false;
    } else {
        $('.shortcutview img').remove();
        $('#val_icon').val('');
    }
});
uploader.on('fileQueued', (file) => {
    let $img = $('<img>');
    $('.shortcutview').append($img);
    // 创建缩略图
    uploader.makeThumb(file, (error, src) => {
        if (error) {
            $img.replaceWith('');
            return;
        }
        $img.attr('src', src);
    }, 48, 48);
});
uploader.on('uploadSuccess', (file, cb) => {
    
    if (cb[0].IsSuccess) {
        let urlPart = cb[0].Url;
        HROS.request.post('/desk/modifyicon', { MemberId: $('#memberId').val(), NewIcon: urlPart }, (responseText) => {
            if (responseText.IsSuccess) {
                $('#isIconByUpload').val('1');
                $('.shortcutview img').attr('src', responseText.Model);
                $('#val_icon').val(urlPart);
                uploader.removeFile(file);
            } else {
                NewCrm.fail("图标保存失败");
            }
        });
    } else {
        NewCrm.fail("图标上传失败");
    }
});
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
        NewCrm.fail(HROS.CONFIG.postOnce);
    },
    beforeSubmit: (curform) => {
        NewCrm.loading(HROS.CONFIG.loadingPrompt);
    },
    callback: (responseText) => {
        if (responseText.IsSuccess) {
            NewCrm.success('修改成功');
            setTimeout(() => {
                top.HROS.app.get(() => {
                    top.$.dialog.list['editdialog'].close();
                });
            }, 1000);
        } else {
            NewCrm.fail(responseText.Message);
        }
    }
});
$('input[name="val_isresize"]').change(function () {
    if ($(this).val() === '1') {
        $('.input-label-isopenmax').slideDown();
    } else {
        $('.input-label-isopenmax').slideUp();
    }
});
//选择应用图片
$('.shortcut-selicon a').click(function () {
    $('.shortcutview img').remove();
    $('.shortcutview').append($(this).html());
    $('#val_icon').val($(this).children('img').attr('valsrc')).focusout();
    $('#isIconByUpload').val('0');
});