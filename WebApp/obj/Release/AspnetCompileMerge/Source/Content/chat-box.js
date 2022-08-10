(function ($) {
    $.fn.chatBox = function () {
        var chatURI = $('.chat-box-icon-top').data('host');
        $(this).append('<iframe class="hide-chat-box" id="chat-box-iframe" src="' + chatURI+'"></iframe>');
	};

	$(document).on('click', '.chat-box-icon-top', function () {
		$('#chat-box-iframe').toggleClass('hide-chat-box');
	})
	$(document).keyup(function (e) {
		if (e.key === "Escape") {
			$('#chat-box-iframe').addClass('hide-chat-box');
		}
	});
})(jQuery);