// let f = $('#form').Validform({
//     btnSubmit: '#btn-submit',
//     postonce: true,
//     showAllError: true,
//     tiptype: (msg, o) => {
//         if (!o.obj.is('form')) {
//             let B = o.obj.parents('form-control');
//             let T = B.children('.help-inline');
//             if (o.type === 2) {
//                 T.text('');
//             } else {
//                 alert(msg);
//             }
//         }
//     },
//     ajaxPost: true,
//     postonceTip: () => {
//         NewCrm.fail(HROS.CONFIG.postOnce);
//     },
//     beforeSubmit: (curform) => {
//         NewCrm.loading(HROS.CONFIG.loadingPrompt);
//     },
//     callback: (responseText) => {
//         let response = responseText;
//         if (response.IsSuccess) {
//             location.href = '/desk/index';
//         } else {
//             NewCrm.fail(response.Message);
//         }
//     }
// });

$('#btn-submit').on('click', () => {
    HROS.request.post('/desk/landing',
        {
            userName: $('#UserName').val(),
            password: $('#Password').val(),
            remember: $('#remember').val()
        }, function (responseText) {
            let response = responseText;
            if (response.IsSuccess) {
                let model = response.Model;
                HROS.localStorage.set("Token", model.Token)
                HROS.cookie.set("User", JSON.stringify({
                    UserFace: model.UserFace,
                    Name: model.Name,
                    Id: model.Id,
                    IsAdmin: model.IsAdmin
                }));
                location.href = `/desk/index`;
            } else {
                NewCrm.fail(response.Message);
            }
        })
})



$(document).keyup((e) => {
    let key = e.which;
    if (key == 13) {
        $('#btn-submit').click();
    }
})
