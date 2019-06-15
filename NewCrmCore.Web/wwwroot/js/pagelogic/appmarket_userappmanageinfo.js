/*<input type="hidden" id="uploadUrl" value="@(NewCrmCore.Infrastructure.Appsetting.UploadUrl)">
    <input type="hidden" id="userId" value="@userId">
    <input type="hidden" id="appId" value='@Context.Request.Query["AppId"]'> */

let uploader = WebUploader.create({
    // 选完文件后，是否自动上传。
    auto: true,
    // swf文件路径
    swf: '../js/webuploader-0.1.5/Uploader.swf',
    // 文件接收服务端。
    server: $('#uploadUrl').val(),
    // 选择文件的按钮。可选。
    // 内部根据当前运行是创建，可能是input元素，也可能是flash.
    pick: {
        id: '#upload',
        multiple: false
    },
    // 只允许选择图片文件。
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
        alert('文件大于300Kb，请压缩后再上传');
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
    debugger
    if (cb[0].IsSuccess) {
        let urlPart = cb[0].Url;
        let appId = $('#appId').val();
        if (parseInt(appId) === 0) {
            $('#isIconByUpload').val('1');
            $('.shortcutview img').attr('src', $('#fileUrl').val() + urlPart)
            $('#val_icon').val(urlPart);
        } else {
            HROS.request.post('/appmarket/modifyicon', { AppId: appId, NewIcon: urlPart }, (responseText) => {
                if (responseText.IsSuccess) {
                    $('#isIconByUpload').val('1');
                    $('.shortcutview img').attr('src', responseText.Model);
                    $('#val_icon').val(urlPart);
                    uploader.removeFile(file);
                } else {
                    NewCrm.msgbox.fail("图标保存失败");
                }
            });
        }
    } else {
        NewCrm.msgbox.fail("图标上传失败");
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
        NewCrm.msgbox.fail(HROS.CONFIG.postOnce);
    },
    beforeSubmit: (curform) => {
        NewCrm.msgbox.loading(HROS.CONFIG.loadingPrompt);
    },
    callback: (responseText) => {
        if (responseText.IsSuccess) {
            if ($('input[name="id"]').val() !== '') {
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
$('input[name="val_type"]').change(function () {
    if ($(this).val() === 'app') {
        $('.input-label-isresize, .input-label-isopenmax, .input-label-isflash, .input-label-issetbar').slideDown();
    } else {
        $('input[name="val_isresize"]').each(() => {
            if ($(this).val() === '1') {
                $(this).prop('checked', true);
            }
        });
        $('input[name="val_isopenmax"]').each(() => {
            if ($(this).val() === '0') {
                $(this).prop('checked', true);
            }
        });
        $('input[name="val_isflash"]').each(() => {
            if ($(this).val() === '0') {
                $(this).prop('checked', true);
            }
        });
        $('.input-label-isresize, .input-label-isopenmax, .input-label-isflash, .input-label-issetbar').slideUp();
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
    $('#isIconByUpload').val('0');
    $('.shortcutview img').remove();
    $('.shortcutview').addClass('bgnone').append($(this).html());
    $('#val_icon').val($(this).children('img').attr('valsrc')).focusout();
});

$('#btn-preview').on('click', () => {
    if (f.check()) {
        if ($('input[name="val_type"]:checked').val() === 'app') {
            top.HROS.window.createTemp({
                title: $('input[name="val_name"]').val(),
                url: $('input[name="val_url"]').val(),
                width: $('input[name="val_width"]').val(),
                height: $('input[name="val_height"]').val(),
                isresize: $('input[name="val_isresize"]:checked').val() === 1,
                isopenmax: $('input[name="val_isopenmax"]:checked').val() === 1,
                isflash: $('input[name="val_isflash"]:checked').val()
            });
        } else {
            top.HROS.widget.createTemp({
                url: $('input[name="val_url"]').val(),
                width: $('input[name="val_width"]').val(),
                height: $('input[name="val_height"]').val()
            });
        }
    } else {
        $.dialog({
            icon: 'error',
            content: '应用无法预览，请讲内容填写完整后再尝试预览'
        });
    }
});


$('#btn-pass').on('click', function () {
    let appid = $(this).attr('appid');
    $.dialog({
        id: 'del',
        content: '确认审核通过该应用？',
        ok: () => {
            HROS.request.post('/appmanager/pass', { appId: appid }, (responseText) => {
                if (responseText.IsSuccess) {
                    window.parent.closeDetailIframe(() => {
                        window.parent.$('#pagination').trigger('currentPage');
                    });
                } else {
                    NewCrm.msgbox.fail(responseText.Message);
                }
            });

        },
        cancel: true
    });
});
$('#btn-unpass').on('click', function () {
    let appid = $(this).attr('appid');
    $.dialog({
        id: 'del',
        content: '确认不通过该应用？',
        ok: () => {
            HROS.request.post('/appmanager/deny', { appId: appid }, (responseText) => {
                if (responseText.IsSuccess) {
                    window.parent.closeDetailIframe(() => {
                        window.parent.$('#pagination').trigger('currentPage');
                    });
                } else {
                    NewCrm.msgbox.fail(responseText.Message);
                }
            });
        },
        cancel: true
    });
});

$('#btn-release').on('click', function () {
    let appid = $(this).attr('appid');
    $.dialog({
        id: 'del',
        content: '确认发布该应用？',
        ok: () => {
            HROS.request.post('/appmarket/release', { appId: appid }, (responseText) => {
                if (responseText.IsSuccess) {
                    window.parent.closeDetailIframe(() => {
                        window.parent.$('#pagination').trigger('currentPage');
                    });
                } else {
                    NewCrm.msgbox.fail(responseText.Message);
                }
            });
        },
        cancel: true
    });
});