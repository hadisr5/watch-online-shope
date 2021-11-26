/*==========================================================
  Theme Name: Nelluce - Responsive HTML5 News Blog Template
  Version: 1.0
  Author: OG Web Solutions
==========================================================*/

(function ($) {
	"use strict";

	/*=============================
	    Preloader
	=============================*/

	$(window).on("load", function () {
		$(".preloader").delay(200).fadeOut("slow");
	});

	/*=============================
	    Trending News
	=============================*/

	$('#trending-news-slider').slick({
		infinite: true,
		arrows: true,
		dots: false,
		rtl: true,
		autoplay: true,
		autoplaySpeed: 5000,
		prevArrow: '<button class="slick-prev slick-arrow" aria-label="Previous" type="button"><i class="fa fa-angle-left"></i></button>',
		nextArrow: '<button class="slick-next slick-arrow" aria-label="Next" type="button"><i class="fa fa-angle-right"></i></button>',
		responsive: [{
			breakpoint: 768,
			settings: {

				dots: false,
				arrows: false,
			}
		}, ]
	});

	/*=============================
	    Hero Slider
	=============================*/

	$('#hero-slider').slick({
		infinite: true,
		arrows: true,
		dots: true,
		rtl: true,
		slidesToShow: 4,
		slidesToScroll: 1,
		autoplay: true,
		responsive: [{
			breakpoint: 1500,
			settings: {
				slidesToShow: 3,
			}
		},
		{
			breakpoint: 991,
			settings: {
				slidesToShow: 2,
			}
		}, ]
	});

	/*=============================
	    Recent Post Slider
	=============================*/


	$('#recent-post-slider').slick({
		infinite: true,
		dots: true,
		rtl: true,
		arrows: false,
		autoplay: true,
	});

	/*=============================
	    Layout Slider
	=============================*/

	$('.layout-01-slider,.layout-02-slider').slick({
		infinite: true,
		dots: false,
		rtl: true,
		arrows: true,
		autoplay: false,
		prevArrow: '<button class="slick-prev slick-arrow" aria-label="Previous" type="button"><i class="fa fa-angle-left"></i></button>',
		nextArrow: '<button class="slick-next slick-arrow" aria-label="Next" type="button"><i class="fa fa-angle-right"></i></button>',
	});

	$('.layout-03-slider').slick({
		infinite: true,
		dots: false,
		rtl: true,
		arrows: true,
		autoplay: false,
		slidesToShow: 3,
		slidesToScroll: 1,
		prevArrow: '<button class="slick-prev slick-arrow" aria-label="Previous" type="button"><i class="fa fa-angle-left"></i></button>',
		nextArrow: '<button class="slick-next slick-arrow" aria-label="Next" type="button"><i class="fa fa-angle-right"></i></button>',
		responsive: [{
			breakpoint: 992,
			settings: {
				slidesToShow: 2,
			}
		},]
	});

	/*=============================
	    Search Form
	=============================*/

	function openSearch() {
		$('.search-area').addClass('active');
		$('.search-form .form-control').focus();
	}

	function closeSearch() {
		$('.search-area').removeClass('active');
		$('.search-form .form-control').blur();
		$('.search-form .form-control').val();
	}

	$('.search a').click(function () {
		openSearch();
	});

	$('#search-close').click(function () {
		closeSearch();
	});

	$(document).keyup(function (e) {
		if (e.keyCode === 27) {
			closeSearch();
		}
	});


})(jQuery);