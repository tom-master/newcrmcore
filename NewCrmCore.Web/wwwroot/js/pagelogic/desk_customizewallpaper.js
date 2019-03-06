NewCrm.Desk.CustomizeWallpaper = {
    url: '',
    userId: ''
};

(() => {
    HROS.request.get("/desk/getuploadwallpapers", {}, (responseText) => {
        if (responseText.IsSuccess) {
            let liElement = '';
            $.each(responseText.Model, (k, v) => {
                liElement += '<li id="' + v.Id + '" style="background:url(' + v.Url + ')"><a href="###">删 除</a></li>';
            });
            $('.view').children('ul').append(liElement);
        } else {
            NewCrm.msgbox.fail(responseText.Message);
        }
    });
})();

var upload = WebUploader.create({
    // 选完文件后，是否自动上传。
    auto: true,
    // swf文件路径
    swf: '~/js/webuploader-0.1.5/Uploader.swf',
    // 文件接收服务端。
    server: NewCrm.Desk.CustomizeWallpaper.url,
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
        userId: NewCrm.Desk.CustomizeWallpaper.userId,
        uploadtype: 'wallpaper'
    }
});
upload.on('beforeFileQueued', (file) => {
    if (file.size > 1000 * 1024) {
        NewCrm.msgbox.info('文件大于1000Kb');
        return false;
    }
});
upload.on('uploadSuccess', (file, cb) => {
    if (cb[0].IsSuccess) {
        let model = {
            Title: cb[0].Title,
            Width: cb[0].Width,
            Height: cb[0].Height,
            Url: cb[0].Url,
            Source: 3,
            UserId: NewCrm.Desk.CustomizeWallpaper.userId,
            Md5: cb[0].Md5,
            ShortUrl: ""
        };

        HROS.request.post('/desk/uploadwallpaper', { wallpaper: model }, (responseText) => {
            if (responseText.IsSuccess) {
                $('.wapppapercustom .view ul').append('<li id="' + responseText.Model.Id + '" style="background-image:url(' + responseText.Model.Url + ')"><a href="###">删 除</a></li>');
                top.HROS.wallpaper.update($('#wallpapertype').val(), responseText.Model.Id);
                upload.removeFile(file);
            } else {
                NewCrm.msgbox.fail(responseText.Message);
            }
        });
    } else {
        NewCrm.msgbox.fail(cb[0].Message);
    }
});

$('#wallpapertype').on('change', () => {
    top.HROS.wallpaper.update($('#wallpapertype').val(), '');
});

$('.wapppapercustom .view').on('click', 'li', function () {
    top.HROS.wallpaper.update($('#wallpapertype').val(), $(this).attr('id'));
});

$('.wapppapercustom .view').on('click', 'li a', function (event) {
    event.stopPropagation();
    let id = $(this).parent().attr('id');
    HROS.request.post("/Desk/RemoveWallpaper", { wallPaperId: id }, (responseText) => {
        if (responseText.IsSuccess) {
            NewCrm.msgbox.success('壁纸移除成功');
            $('#' + id).remove();
        } else {
            NewCrm.msgbox.fail(responseText.Message);
        }
    });
});

$('#webWallpaper').on('click', () => {
    let webUrl = $('#wallpaperurl').val();
    if (!webUrl) {
        NewCrm.msgbox.info('请输入一个合法的url地址');
    } else {
        HROS.request.post("/desk/webwallpaper", { webUrl: webUrl }, (responseText) => {
            if (parseInt(responseText.Model.Item1) === 0) {
                NewCrm.msgbox.fail('未能获取到所指定的网络图片');
            } else {
                top.HROS.wallpaper.update($('#wallpapertype').val(), parseInt(responseText.Model.Item1));
            }
        });
    }
});

