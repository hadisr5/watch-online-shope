$(document).ready(function () {

    if ($('.suggestion-slider').length) {
        var autoplay = 5000;
        var mySwiper = new Swiper('.swiper-container', {
            el: '.suggestion-slider',
            loop: true,
            slidesPerView: 1,
            watchSlidesProgress: true,
            autoplay: {
                delay: autoplay,
            },
            onProgress: move()
        });

        function move() {
            var elem = document.getElementById("progress");
            var width = 0;
            var autoplayTime = autoplay / 100;
            var id = setInterval(frame, autoplayTime);

            function frame() {
                if (width >= 100) {
                    elem.style.width = 0;
                    clearInterval(id);
                    move();
                } else {
                    width++;
                    elem.style.width = width + '%';
                }
            }
        }
    }

    if ($('.swiper-container').length) {

        var mySwiper = new Swiper('.swiper-container', {
            el: '.swiper-md-slider',
            slidesPerView: 4,
            autoplay: false,
            navigation: {
                nextEl: '.swiper-button-next',
                prevEl: '.swiper-button-prev',
            },
            breakpoints: {
                992: {
                    slidesPerView: 3,
                },
                768: {
                    slidesPerView: 3.5,
                    scrollbar: {
                        el: '.swiper-scrollbar',
                        hide: true,
                    },
                    spaceBetween: 10,
                },
                576: {
                    slidesPerView: 2.5,
                    scrollbar: {
                        el: '.swiper-scrollbar',
                        hide: true,
                    },
                    spaceBetween: 10,
                },
                480: {
                    slidesPerView: 1.9,
                    scrollbar: {
                        el: '.swiper-scrollbar',
                        hide: true,
                    },
                    spaceBetween: 8,
                }
            }
        });

        var ordersSwiper = new Swiper('.profile-order-steps', {
            slidesPerView: 4,
            spaceBetween: 0,
            navigation: {
                nextEl: '.profile-order-steps-button--next',
                prevEl: '.profile-order-steps-button--prev'
            }
        });

        if (!!ordersSwiper) {
            for (var i = 0; i < ordersSwiper.length; ++i) {
                ordersSwiper[i].slideTo($(ordersSwiper[i].$el).closest('.profile-order').find('.profile-order-steps div.is-active').index() - 5);
            }
        }

        var mySwiper = new Swiper('.swiper-container', {
            el: '.swiper-lg-slider',
            slidesPerView: 5,
            autoplay: false,
            navigation: {
                nextEl: '.swiper-button-next',
                prevEl: '.swiper-button-prev',
            },
            breakpoints: {
                1400: {
                    slidesPerView: 5,
                },
                1200: {
                    slidesPerView: 4,
                },
                992: {
                    slidesPerView: 3,
                },
                768: {
                    slidesPerView: 3.5,
                    scrollbar: {
                        el: '.swiper-scrollbar',
                        hide: true,
                    },
                    spaceBetween: 10,
                },
                576: {
                    slidesPerView: 2.5,
                    scrollbar: {
                        el: '.swiper-scrollbar',
                        hide: true,
                    },
                    spaceBetween: 10,
                },
                480: {
                    slidesPerView: 1.9,
                    scrollbar: {
                        el: '.swiper-scrollbar',
                        hide: true,
                    },
                    spaceBetween: 8,
                }
            }
        });



        var mySwiper = new Swiper('.swiper-container', {
            el: '#gallery-slider',
            slidesPerView: 4,
            autoplay: false,
            navigation: {
                nextEl: '.gallery-button-next-id',
                prevEl: '.gallery-button-prev-id',
            }
        });

    }



    $('.back-to-top').click(function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: 0
        }, 800, 'easeInExpo');
    });

    if ($("#img-product-zoom").length) {
        $("#img-product-zoom").ezPlus({
            containLensZoom: true,
            gallery: 'gallery_01f',
            cursor: "crosshair",
            galleryActiveClass: "active",
            responsive: true,
            imageCrossfade: true,
            zoomWindowPosition: 9,
            zoomWindowFadeIn: 500,
            zoomWindowFadeOut: 500,
            respond: [
                {
                    zoomType: "inner",
                    range: '600-799',
                    tintColour: '#F00',
                    zoomWindowPosition: 0,
                    enabled: false,
                    showLens: false
                },
                {
                    zoomType: "inner",
                    range: '800-1199',
                    tintColour: '#00F',
                    zoomWindowPosition: 0
                },
                {
                    range: '100-599',
                    enabled: false,
                    showLens: false
                }
            ]
        });
    }

    $(".tab-content .sum-more").click(function () {
        var sumaryBox = $(this).parents('.parent-expert');
        sumaryBox.find('.content-expert').toggleClass('active');
        sumaryBox.find('.shadow-box').fadeToggle();

        $(this).find('i').toggleClass('active');

        $(this).find('.show-more').fadeToggle(0);
        $(this).find('.show-less').fadeToggle(0);

    });

    $(".product-variant .more-color-product").click(function () {
        var sumaryBox = $(this).parents('.product-variant');
        sumaryBox.find('ul.product-variants').toggleClass('active');

        $(this).find('.show-more').fadeToggle(0);
        $(this).find('.show-less').fadeToggle(0);

    });

    $(".seller-list-section .more-seller-list").click(function () {
        var sumaryBox = $(this).parents('.seller-list-section');
        sumaryBox.find('.table-suppliers').toggleClass('active');

        $(this).find('.show-more').fadeToggle(0);
        $(this).find('.show-less').fadeToggle(0);

    });



    $(".product-params .sum-more").click(function () {
        var sumaryBox = $(this).parents('.product-params');
        sumaryBox.toggleClass('active');

        $(this).find('i').toggleClass('active');

        $(this).find('.show-more').fadeToggle(0);
        $(this).find('.show-less').fadeToggle(0);

    });

    $('nav.header-responsive li.active').addClass('open').children('ul').show();

    $("nav.header-responsive li.sub-menu> a").on('click', function () {
        $(this).removeAttr('href');
        var e = $(this).parent('li');
        if (e.hasClass('open')) {
            e.removeClass('open');
            e.find('li').removeClass('open');
            e.find('ul').slideUp(400);

        } else {
            e.addClass('open');
            e.children('ul').slideDown(400);
            e.siblings('li').children('ul').slideUp(400);
            e.siblings('li').removeClass('open');
        }
    });

    // Start scroll

    $(window).scroll(function () {
        if ($(this).scrollTop() > 60) {
            $("nav.header-responsive").css({
                height: '60px'
            });
            $("nav.header-responsive .search-nav").css({
                opacity: '0',
                visibility: 'hidden'
            });
        } else {
            $("nav.header-responsive").css({
                height: '110px'
            });
            $("nav.header-responsive .search-nav").css({
                opacity: '1',
                visibility: 'visible'
            });
        }
    });

    // End scroll

    // favorites product

    $("ul.gallery-options button.add-favorites").on("click", function () {
        $(this).toggleClass("favorites");
    });

    // favorites product

    // LocalSearch
    if ($("header.main-header").length) {
        $("#gsearchsimple").focus(function () {
            $("header.main-header").css({
                "position": "fixed",
                "top": "0",
                "right": "0"
            });
            $("main.main").css({
                "margin-top": "136px"
            });
            $("header.main-header .search-area form.search ul.search-box-list").css("display", "block");
            $(".overlay-search-box").css({
                "opacity": "1",
                "visibility": "visible"
            });
        });
        $("nav.header-responsive form input").focus(function(){
            $("nav.header-responsive form").addClass("full-width");
        });
        $("#gsearchsimpleMobile").focus(function () {
            $("#resultsMobile").css("display", "block");
            $("button.closeResultSearchMobile").css("display", "block");
        });
        $("button.closeResultSearchMobile").on("click",function(){
            $("#resultsMobile").css("display", "none");
            $(this).css("display","none");
            $("nav.header-responsive form").removeClass("full-width");
        });
        $(".overlay-search-box,header.main-header .col-md-4.col-sm-12,header.main-header .col-lg-2.col-md-3.col-sm-4.col-5,header.main-header .main-menu").click(function () {
            $("header.main-header").css({
                "position": "relative"
            });
            $("main.main").css({
                "margin-top": "0"
            });
            $("header.main-header .search-area form.search ul.search-box-list").css("display", "none");
            $(".overlay-search-box").css({
                "opacity": "0",
                "visibility": "hidden"
            });
        });
        $(".localSearchSimple").jsLocalSearch({
            action: "Show",
            html_search: true,
            mark_text: 'marktext'
        });
        $("#search-form-dl").submit(function(e){
            e.preventDefault();
            var searchFormInput = $("#gsearchsimple").val();
            if (searchFormInput.length < 1) {
                toastr.warning('لطفا کلمه ای برای جستجو تایپ کنید!', 'اوپس...');
            } else {
                $(this).unbind('submit').submit()
            }
        });
        $("#search-form-dl-res").submit(function(e){
            e.preventDefault();
            var searchFormInput = $("#gsearchsimpleMobile").val();
            if (searchFormInput.length < 1) {
                toastr.warning('لطفا کلمه ای برای جستجو تایپ کنید!', 'اوپس...');
            } else {
                $(this).unbind('submit').submit()
            }
        });
    }
    // LocalSearch

    // menu
    var wid = 0;
    var $hoverEffect = $('.js-nav-list-category-hover'),
        $headerLinks = $('header.main-header .menu > ul > li');
    var moveHover = function (self) {
        var parent = self
            .parent()
            .parent()
            .parent();
        var widparent = self.children(".product-category").children(".dropdownmenu");
        wid = widparent.width() ;
        if (wid == 0 || wid== undefined) {
            widparent = self;
            wid = widparent.width();
        }
        wid +=  parseInt(widparent.css('padding-right').replace('px', ''));
        $hoverEffect
            .css('width', wid)
            .css(
                'right',
                parent.width() -
                (self.offset().left + self.width()) +
                parent.offset().left 
            );
        $hoverEffect.css('transform', 'scaleX(1)');
    };

    var removeHover = function () {
        $hoverEffect.css('transform', 'scaleX(0)');
    };

    $headerLinks.hover(function () {
            moveHover.call(this, $(this));
        },
        function () {
            removeHover.call(this, $(this));
        });

    $('header.main-header .menu .parent-category ul').find('> li:eq(0)').addClass('active');
    $('header.main-header .menu ul li.category-products').hover(function(){
        $('header.main-header .search-area').removeClass('active');
        $('.nav-categories-overlay').addClass('active');
    },function(){
        $('.nav-categories-overlay').removeClass('active');
    });
    $('header.main-header .menu .parent-category li a').on('mouseenter',function (g) {
        var tab = $(this).closest('.product-category'),
            index = $(this).closest('li').index();

        tab.find('.parent-category ul > li').removeClass('active');
        $(this).closest('li').addClass('active');

        tab.find('.children-categories').find('.children-category').not('children-category:eq(' + index + ')').fadeOut(0);
        tab.find('.children-categories').find('.children-category:eq(' + index + ')').fadeIn(0);

        g.preventDefault();
    } );
    // menu

    // verify-phone-number
    if ($("#countdown-verify-end").length) {
        var $countdownOptionEnd = $("#countdown-verify-end");

        $countdownOptionEnd.countdown({
            date: (new Date()).getTime() + 180 * 1000, // 1 minute later
            text: '<span class="day">%s</span><span class="hour">%s</span><span>: %s</span><span>%s</span>',
            end: function () {
                $countdownOptionEnd.html("<a href='' class='btn-link-border form-account-link'>ارسال مجدد</a>");
            }
        });

        $(".line-number").keyup(function () {
            $(this).next().focus();
        });
    }
    // verify-phone-number

    /* **************  drilldown-menu */
    if ($("#product-seller-info").length) {
        $("#product-seller-info").stackMenu();
    }

    // gallery-carousel
    if ($(".carousel.gallery-carousel").length) {
        $('.carousel.gallery-carousel').carousel({
            interval: false
        });
    }

    /* sidebar-sticky */
    if ($('.container .sticky-sidebar').length) {
        $('.container .sticky-sidebar').theiaStickySidebar();
    }

    $('#btn-product-seller-list').click(function (e) {
        e.preventDefault();
        var target = $($(this).attr('href'));
        if (target.length) {
            var scrollTo = target.offset().top;
            $('body, html').animate({
                scrollTop: scrollTo + 'px'
            }, 800);
        }
    });

    /* select2 */
    $('.selectpicker').each(function () {
        $(this).select2({
            dir: "rtl",
            placeholder: $(this).attr('placeholder'),
            allowClear: Boolean($(this).data('allow-clear')),
            "language": {
                "noResults": function () {
                    return "جستجو کن";
                }
            }
        });
    });

    $("#btn-checkout-contact-location").click(function () {
        $(".checkout-address").addClass("show");
        $(".checkout-contact-content").addClass("hidden");
    });

    $("#cancel-change-address-btn").click(function () {
        $(".checkout-address").removeClass("show");
        $(".checkout-contact-content").removeClass("hidden");
    });

    $('input[name=bank_id]').change(function () {
        $('input[name=bank_id]').closest('label').removeClass('is-selected');
        $(this).closest('label').addClass('is-selected');
    });

    /* gallery-modal */
    if($(".remodal-gallery-main-id").length) {
        var i = $(".remodal-gallery-main-id .swiper-slide"),
            n = new Swiper(".remodal-gallery-main-id", {
                effect: "fade",
                zoom: true,
                fadeEffect: {
                    crossFade: !0
                },
                on: {
                    init: function () {
                        var a = this.activeIndex;
                        $(".remodal-gallery-main-id .swiper-slide").eq(a).hasClass("c-remodal-gallery__3d-slide") ? $(".swiper-pagination").hide() : $(".swiper-pagination").show()
                    },
                    slideChange: function () {
                        var a = this.activeIndex;
                        i.eq(a).css("z-index", "2"), i.not(i.eq(a)).css("z-index", "-1"), i.eq(a).hasClass("remodal-gallery-3d-slide") ? $(".swiper-pagination").hide() : $(".swiper-pagination").show(), dispatchEvent(new Event("scroll"))
                    }
                },
                observer: !0,
                observeParents: !0
            });
        i.eq(0).css("z-index", "2"), i.not(i.eq(0)).css("z-index", "-1");
        var r = new Swiper(".modal-gallery-thumbs-id", {
            direction: "vertical",
            slideToClickedSlide: !0,
            spaceBetween: 10,
            keyboard: {
                enabled: !0,
                onlyInViewport: !0
            },
            slidesPerView: "auto",
            touchRatio: .2,
            centeredSlides: !0,
            navigation: {
                nextEl: ".modal-button-next-id",
                prevEl: ".modal-button-prev-id"
            },
            on: {
                slideChange: function () {
                    dispatchEvent(new Event("scroll"))
                }
            },
            observer: !0,
            observeParents: !0,
            breakpoints: {
                768: {
                    direction: "horizontal",
                    slidesPerView: 4,
                },
            }
        });
        n.controller.control = r;
        r.controller.control = n;
    }
    /* gallery-modal */
    if($(".skeleton-loader-product-card").length) {
        $(".skeleton-loader-product-card").fadeOut(0);
    }
    if($(".skeleton-loader").length) {
        $(".skeleton-loader").fadeOut(0);
    }
    if($(".listing-items .product-box").length) {
        $(".listing-items .product-box").fadeIn(0);
    }
    if($("#product-seller-info").length) {
        $("#product-seller-info").fadeIn(0);
    }
});