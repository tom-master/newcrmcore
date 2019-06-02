$("#wallpapertype").val(top.HROS.CONFIG.wallpaperType.toLowerCase());
$("#wallpapertype").on('change', () => {
    top.HROS.wallpaper.update($('#wallpapertype').val(), '');
});

$('.wallpaper li').on('click', function () {
    top.HROS.wallpaper.update($('#wallpapertype').val(), $(this).attr('wpid'));
});