//ajax请求
function ajaxGetData(obj) {
    $.ajax({
        url: obj.links,
        data: obj.words,
        type: "post",
        success: obj.successed,
        error: obj.error
    });
}

var $dyplayer = $('.dyplayer');
var $dyplayerSwitch = $(".dyplayer-switch");
var $dyplayerSwitchIcon = $(".dyplayer-switch>i");
    $dyplayerSwitch.on('click', function () {
    var switchStatus = $dyplayerSwitch.attr('data-switch');
    if (switchStatus == "on") {
        switchStatus = "off";
        $dyplayer.removeClass('on').addClass('off');
        $dyplayerSwitchIcon.removeClass('fa-angle-left').addClass('fa-angle-right');
    } else {
        switchStatus = "on";
        $dyplayer.removeClass('off').addClass('on');
        $dyplayerSwitchIcon.removeClass('fa-angle-right').addClass('fa-angle-left');
    }
    $dyplayerSwitch.attr('data-switch', switchStatus);
});




