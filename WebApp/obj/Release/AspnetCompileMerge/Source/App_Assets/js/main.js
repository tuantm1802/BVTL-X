(function ($) {
    menuActive();
    window.onload = function () {
        $(document).ready(function () {		
			moblie_bar();
            fill_potier();
            multi_acrodition();
            show_list_button();
            changes_info_modal();
            height_nav_box();
            adobe_in_nav();
            fill_potier_tree();
            new WOW().init();
            $("#resizable").resizable();
            width_to_nav_consert();
            mouse_over_action_s();
        });
    };
})(jQuery);


function menuActive() {
    var $level2 = $('.header-bottom ul.nav-menu li li.active');
    if ($level2.length > 0)
        $level2.parent().parent().addClass('active');
    var $level3 = $('.header-bottom ul.nav-menu li li li.active');
    if ($level3.length > 0) {
        $level2 = $level3.parent().parent();
        $level2.addClass('active');
        $level2.parent().parent().addClass('active');
    }
}
function moblie_bar() {
    var $main_nav = $('#main-nav');
    var $toggle = $('.toggle');

    var defaultData = {
        maxWidth: false,
        customToggle: $toggle,
        // navTitle: 'All Categories',
        levelTitles: true,
        pushContent: '#container'
    };

    // add new items to original nav
    $main_nav.find('li.add').children('a').on('click', function() {
        var $this = $(this);
        var $li = $this.parent();
        var items = eval('(' + $this.attr('data-add') + ')');

        $li.before('<li class="new"><a>' + items[0] + '</a></li>');

        items.shift();

        if (!items.length) {
            $li.remove();
        } else {
            $this.attr('data-add', JSON.stringify(items));
        }

        Nav.update(true);
    });

    // call our plugin
    var Nav = $main_nav.hcOffcanvasNav(defaultData);

    // demo settings update

    const update = (settings) => {
        if (Nav.isOpen()) {
            Nav.on('close.once', function() {
                Nav.update(settings);
                Nav.open();
            });

            Nav.close();
        } else {
            Nav.update(settings);
        }
    };

    $('.actions').find('a').on('click', function(e) {
        e.preventDefault();

        var $this = $(this).addClass('active');
        var $siblings = $this.parent().siblings().children('a').removeClass('active');
        var settings = eval('(' + $this.data('demo') + ')');

        update(settings);
    });

    $('.actions').find('input').on('change', function() {
        var $this = $(this);
        var settings = eval('(' + $this.data('demo') + ')');

        if ($this.is(':checked')) {
            update(settings);
        } else {
            var removeData = {};
            $.each(settings, function(index, value) {
                removeData[index] = false;
            });

            update(removeData);
        }
    });
}

function fill_potier(){
    var btn_select = document.querySelectorAll(".ac-fill .title");
    if(btn_select == null){
        return 0;
    }

    else{
        var drop_down = document.querySelectorAll(".ac-fill .drop-down");
        for(var i = 0; i < btn_select.length; i++){
            btn_select[i].addEventListener("click", function(){
                this.classList.toggle("active");
                for(var j = 0; j < drop_down.length; j++){
                    var pannel = drop_down[j].previousElementSibling;
                    if(pannel.classList[1] == 'active'){
                        drop_down[j].classList.add('active');
                    }
                    else{
                        drop_down[j].classList.remove('active');
                    }
                    
                }
            })
        }
        

        var select_li = document.querySelectorAll(".ac-fill .drop-down .list li");
        for(var i = 0; i < select_li.length; i++){
            select_li[i].addEventListener("click", function(){
                var content = this.innerHTML;
                for(var j = 0; j < drop_down.length; j++){
                    var pannel = drop_down[j].previousElementSibling;
                    if(pannel.classList[1] == 'active'){
                        pannel.innerHTML = content;
                        pannel.classList.remove("active");
                        drop_down[j].classList.remove("active");
                    }
                }
            })
        }
    }
}

function fill_potier_tree(){
    var btn_select = document.querySelectorAll(".fill-tree .title");
    if(btn_select == null){
        return 0;
    }

    else{
        var drop_down = document.querySelectorAll(".fill-tree .drop-down");
        for(var i = 0; i < btn_select.length; i++){
            btn_select[i].addEventListener("click", function(){
                this.classList.toggle("active");
                for(var j = 0; j < drop_down.length; j++){
                    var pannel = drop_down[j].previousElementSibling;
                    if(pannel.classList[1] == 'active'){
                        drop_down[j].classList.add('active');
                    }
                    else{
                        drop_down[j].classList.remove('active');
                    }
                    
                }
            })
        }
        
    }
}

function adobe_in_nav(){
    var nav_tabs = $(".nav-tabs-box .nav-tabs.tab-main");
    var nav_tabs_box = $(".nav-box .nav-tabs-box");
    var li_1 = $(".nav-tabs.tab-main .nav-item");
    
    var width_1 = nav_tabs.width();
    var width_2 = nav_tabs_box.width();
    var btn_link = document.getElementById("pick_up_changes_2");

    if(width_1 < width_2){
        btn_link.style.display = "none"; 
    }

    else{
        for(var i = 0; i<li_1.length; i++){
            li_1[i].classList.add("fc-w-to-tab");
        }

        for(var i = 5; i<li_1.length; i++){
            li_1[i].classList.add("over-load-li");
        }

        var btn_link = document.getElementById("pick_up_changes_2");
        if(btn_link == null){
            return 0;
        }
        else{
            var over_load = document.querySelectorAll("li.over-load-li");
            
            var top = 30;
            for(var i = 0; i < over_load.length; i++){
                over_load[i].style.top = top + "px";
                top = top + 35;
            }

            btn_link.addEventListener("click", function(){
                this.classList.toggle("show");
                for(var i = 0; i < over_load.length; i++){
                    over_load[i].classList.toggle("d-block");
                    over_load[i].addEventListener("click", function(e){
                        for(var k = 0; k< over_load.length; k ++){
                            over_load[k].classList.add("over-load-li");
                            over_load[k].classList.remove("d-block");
                            btn_link.classList.remove("show");
                        }
                        this.classList.remove("over-load-li");
                        // this.classList.add("d-block");
                        
                    })
                }
            })
        }
        for(var i = 0; i<li_1.length; i++){
            li_1[i].addEventListener("click", function(){
                for(var i = 5; i<li_1.length; i++){
                    li_1[i].classList.add("over-load-li");
                }
            })
        }
    }

}

function multi_acrodition(){
	var accordion = (function(){
  	var $accordion = $('.js-accordion');
	var $accordion_header = $accordion.find('.js-accordion-header');
	var $accordion_item = $('.js-accordion-item');
	   
		// default settings 
		var settings = {
		  // animation speed
		  speed: 400,
		  
		  // close all other accordion items if true
		  oneOpen: false
		};
		  
		return {
		  // pass configurable object literal
		  init: function($settings) {
			$accordion_header.on('click', function() {
			  accordion.toggle($(this));
			  
			  setTimeout(() => {
			  }, 400)
			});
			
			$.extend(settings, $settings); 
			
			// ensure only one accordion is active if oneOpen is true
			if(settings.oneOpen && $('.js-accordion-item.active').length > 1) {
			  $('.js-accordion-item.active:not(:first)').removeClass('active');
			}
			
			// reveal the active accordion bodies
			$('.js-accordion-item.active').find('> .js-accordion-body').show();
		  },
		  toggle: function($this) {
				  
			if(settings.oneOpen && $this[0] != $this.closest('.js-accordion').find('> .js-accordion-item.active > .js-accordion-header')[0]) {
			  $this.closest('.js-accordion')
				.find('> .js-accordion-item') 
				.removeClass('active')
				.find('.js-accordion-body')
				.slideUp()
			}
			
			// show/hide the clicked accordion item
			$this.closest('.js-accordion-item').toggleClass('active');
			$this.next().stop().slideToggle(settings.speed);
		  }
		}
	})();

	accordion.init({ speed: 300, oneOpen: true });
}

function show_list_button(){
    var btn = document.getElementById("changes-list-btn");
    if (btn == null){
        return 0;
    }

    else{
        var list_btn = document.querySelector(".list-buttun-ct");
        btn.addEventListener("click", function(){
            btn.classList.toggle("active");
            list_btn.classList.toggle("show");
        })
    }
}

function changes_info_modal(){
    var in_model = document.querySelector("#danh_sach_san_pham");
    if(in_model == null){
        return 0;
    }
    else{
        var btn_c_1 = $("#danh_sach_san_pham .un-check");
        var tbody_2 = document.querySelector("#tbm-2 tbody");
        var tbody_1 = document.querySelector("#tbm-1 tbody");
        var check = true;

        for(var i = 0; i < btn_c_1.length; i++){
            btn_c_1[i].addEventListener("click", function(){
                tbody_2.append(this.closest('tr'));  
                this.classList.remove("un-check");   
                this.classList.toggle("checked");          

                if(this.classList[0] == null){
                    tbody_1.append(this.closest('tr'));  
                    this.classList.add("un-check");  
                }
            })
        }
    }
}

function height_nav_box(){
    var form_height = $(".form-infor.firts-to-h").height();
    var nav_height = $(".nav-box").height();
    var conent_height = $(".content-system").height();
    var action_height = $(".action-system").height();
    nav_height = action_height - form_height - 20;
    $(".nav-box").height(nav_height);
    var height_to_2 = action_height - form_height - 90;
    $(".nav-box.spe-height").height(height_to_2);
    
}

function width_to_nav_consert(){
    
    $('.action-system').bind('resize', function(){
        var nav_width = $(".action-system").width();
        if(nav_width <= 70){
            $(".action-system .nav-tabs").addClass("d-none");
            $(".action-system .container").addClass("d-none");
            $("#btn-changes-s-nav").addClass("active");
            $(".action-system").addClass("console_w");
            $(".fill-box").addClass("transform-cons-rec");
        }
        
        else{
            $(".action-system .nav-tabs").removeClass("d-none");
            $(".action-system .container").removeClass("d-none");
            $("#btn-changes-s-nav").removeClass("active");
            $(".action-system").removeClass("console_w");
            $(".fill-box").removeClass("transform-cons-rec");
            
        }

    });

    $("#btn-changes-s-nav").click(function(){
        $("#btn-changes-s-nav").toggleClass("active");
        var nav_width = $(".action-system").width();

        if($("#btn-changes-s-nav").hasClass("active")){
            $(".action-system").css("width", "60px");
            $(".action-system .nav-tabs").addClass("d-none");
            $(".action-system .container").addClass("d-none");
            $(".action-system").addClass("console_w");
            $(".fill-box").addClass("transform-cons-rec");
        }
        else{
            $(".action-system").css("width", "650px");
            $(".action-system .nav-tabs").removeClass("d-none");
            $(".action-system .container").removeClass("d-none");
            $(".action-system").removeClass("console_w");
            $(".fill-box").removeClass("transform-cons-rec");
        }
    })
}

function mouse_over_action_s(){
    $(".action-system")
    .mouseenter(function() {
        $(this).addClass("bd-left-in");
    })
    .mouseleave(function() {
        $(this).removeClass("bd-left-in");
    });
}
function conver_tvkhongdau(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    return str;
}
$(".date").datepicker({
    autoclose: true,
    todayHighlight: true
}).datepicker();

$("#dateFrom").datepicker({
    autoclose: true,
    todayHighlight: true
}).datepicker();

$("#dateTo").datepicker({
    autoclose: true,
    todayHighlight: true
}).datepicker();

$("#dateFrom_2").datepicker({
    autoclose: true,
    todayHighlight: true
}).datepicker();

$("#dateTo_2").datepicker({
    autoclose: true,
    todayHighlight: true
}).datepicker();



