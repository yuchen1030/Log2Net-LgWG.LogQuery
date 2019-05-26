//Skin changer
function skinChanger() {
    $('.right-sidebar .demo-choose-skin li').on('click', function () {
        var currentTheme = $('.right-sidebar .demo-choose-skin li.active').data('theme');
        $('.right-sidebar .demo-choose-skin li').removeClass('active');

        var $selected = $(this);
        $selected.addClass('active');
        var selectedTheme = $selected.data('theme');

        $('body')
            .removeClass('theme-' + currentTheme)
            .addClass('theme-' + selectedTheme);

        //Change theme settings on the server
        abp.services.app.configuration.changeUiTheme({
            theme: selectedTheme
        });
    });
}

//Skin tab content set height and show scroll
function setSkinListHeightAndScroll() {
    var height = $(window).height() - ($('.navbar').innerHeight() + $('.right-sidebar .nav-tabs').outerHeight());
    var $el = $('.demo-choose-skin');

    $el.slimScroll({ destroy: true }).height('auto');
    $el.parent().find('.slimScrollBar, .slimScrollRail').remove();

    $el.slimscroll({
        height: height + 'px',
        color: 'rgba(0,0,0,0.5)',
        size: '4px',
        alwaysVisible: false,
        borderRadius: '0',
        railBorderRadius: '0'
    });
}

//Setting tab content set height and show scroll
function setSettingListHeightAndScroll() {
    var height = $(window).height() - ($('.navbar').innerHeight() + $('.right-sidebar .nav-tabs').outerHeight());
    var $el = $('.right-sidebar .demo-settings');

    $el.slimScroll({ destroy: true }).height('auto');
    $el.parent().find('.slimScrollBar, .slimScrollRail').remove();

    $el.slimscroll({
        height: height + 'px',
        color: 'rgba(0,0,0,0.5)',
        size: '4px',
        alwaysVisible: false,
        borderRadius: '0',
        railBorderRadius: '0'
    });
}

//Activate notification and task dropdown on top right menu
function activateNotificationAndTasksScroll() {
    $('.navbar-right .dropdown-menu .body .menu').slimscroll({
        height: '254px',
        color: 'rgba(0,0,0,0.5)',
        size: '4px',
        alwaysVisible: false,
        borderRadius: '0',
        railBorderRadius: '0'
    });
}


/*
　 *　方法:Array.remove(dx)
　 *　功能:删除数组元素.
　 *　参数:dx删除元素的下标.
　 *　返回:在原数组上修改数组
*/
//经常用的是通过遍历,重构数组.
Array.prototype.remove = function (dx) {
    if (isNaN(dx) || dx > this.length) { return false; }
    for (var i = 0, n = 0; i < this.length; i++) {
        if (this[i] != this[dx]) {
            this[n++] = this[i]
        }
    }
    this.length -= 1
　}

function clearMailNotifyMsg() {
    if ($('#mailNotifyNumTxt').text() == "0") {
        abp.message.warn("当前没有需要清除的信息");
        return;
    }
    abp.message.confirm(
        '确认要清除所有的告警信息么？',
        function (isConfirmed) {
            if (isConfirmed) {
                $('#mailNotifyNumTxt').text("0");
                $("#mailNotifyDetail").empty();    
                playStopWarningMusic();
            }
        }
    );
}

function removeNotifyList(obj) {
    $('#mailNotifyNumTxt').text(parseInt($('#mailNotifyNumTxt').text()) - 1);
    $(obj).remove();
    playStopWarningMusic();
}


//警告音乐播放停止,参数表示是否必须播放音乐
function playStopWarningMusic(mustDisplay) {
    var audio = document.getElementById("warning-sound");
    if (audio !== null) {
        //检测播放是否已暂停.audio.paused 在播放器播放时返回false.
        console.log(audio.paused);
        if (mustDisplay) {
            $('#warnningMusicDiv').show();
            audio.play();   //播放
        } else //if (!mustDisplay) 
        {
            $('#warnningMusicDiv').hide();
            audio.pause();  //暂停
        }
    }
}


(function ($) {

    //Initialize BSB admin features
    $(function () {
        skinChanger();
        activateNotificationAndTasksScroll();

        setSkinListHeightAndScroll();
        setSettingListHeightAndScroll();
        $(window).resize(function () {
            setSkinListHeightAndScroll();
            setSettingListHeightAndScroll();
        });

        $('#mailClearBtn').click(function () {
            clearMailNotifyMsg();            
        });

        $('#warnHead').click(function () {
            clearMailNotifyMsg();
        });
        

        $('li.mailListCss').click(function () {
            removeNotifyList(this);
        });


        $("#mailNotifyDetail").on("click", "li", function () {
            removeNotifyList(this);
        });


    });

})(jQuery);