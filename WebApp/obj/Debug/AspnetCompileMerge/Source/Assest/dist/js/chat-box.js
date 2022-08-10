(function ($) {

    $.fn.CreateChatBox = function (options) {

        // Default options
        var settings = $.extend({
            width: '300px',
            height: '400px',
        }, options);

        // Apply options
        return this.append('<div id="chat-icon" class="checkbox-botom">' +
            '<button type="button" onclick="ShowHideIframe()" class="btn btn-info btn-circle btn-xl" sty><i class="fa fa-comments"></i></button>' +
            '</div><iframe id="iframe" src="http://27.72.104.236:8881/Chat/Index" class="hide chat-box"></iframe>');

    };
}(jQuery));
function ShowHideIframe() {
    $('#iframe').removeClass('hide');
}

document.onkeydown = function (evt) {
    evt = evt || window.event;
    if (evt.keyCode == 27) {
        $('#iframe').addClass('hide');
    }
};