/*
Name: 			One Page Agency
Written by: 	Okler Themes - (http://www.okler.net)
Theme Version:	12.1.0
*/

(($ => {
    const $window = $(window);

    /*
	* Header
	*/
    const $header = $('#header');

    window.onscroll = () => {
		if (window.pageYOffset > $header.offset().top) {
			$('html').addClass('sticky-header-active');
		} else {
			$('html').removeClass('sticky-header-active');
		}
	};

    /*
	* Collapse Menu Button
	*/
    $('.header-btn-collapse-nav').on('click', () => {
		 $('html, body').animate({
	        scrollTop: $(".header-btn-collapse-nav").offset().top - 18
	    }, 300);
	});

    /*
	* Isotope
	*/
    const $wrapper = $('#itemDetailGallery');

    if( $wrapper.get(0) ) {
		$wrapper.waitForImages(() => {
			$wrapper.isotope({
				itemSelector: '.isotope-item'
			});
		});
	}

    /*
	Load More
	*/
    const portfolioLoadMore = {

		pages: 0,
		currentPage: 1,
		$wrapper: $('#portfolioLoadMoreWrapper'),
		$btn: $('#portfolioLoadMore'),
		$btnWrapper: $('#portfolioLoadMoreBtnWrapper'),
		$loader: $('#portfolioLoadMoreLoader'),

		build() {

			const self = this;

			self.pages = self.$wrapper.data('total-pages');

			if(self.pages <= 1) {

				self.$btnWrapper.remove();
				return;

			} else {

				self.$btn.on('click', () => {
					self.loadMore();
				});

				// Infinite Scroll
				if(self.$btn.hasClass('btn-portfolio-infinite-scroll')) {
					theme.fn.intObs( '#portfolioLoadMore', "$('#portfolioLoadMore').trigger('click');", {
						rootMargin: '0px 0px 0px 0px'
					}, true );
				}

			}

		},
		loadMore() {

			const self = this, ajax_url = ( self.$wrapper.data('ajax-url') ) ? self.$wrapper.data('ajax-url') : 'ajax/portfolio-ajax-load-more-';

			self.$btn.hide();
			self.$loader.addClass('portfolio-load-more-loader-showing').show();

			// Ajax
			$.ajax({
				url: ajax_url + (parseInt(self.currentPage)+1) + '.html',
				complete({responseText}) {

					const $items = $(responseText);

					setTimeout(() => {

						self.$wrapper.append($items)

						self.$wrapper.isotope('appended', $items);

						self.currentPage++;

						if(self.currentPage < self.pages) {
							self.$btn.show().blur();
						} else {
							self.$btnWrapper.remove();
						}

						// Carousel
						$(() => {
							$('[data-plugin-carousel]:not(.manual), .owl-carousel:not(.manual)').each(function() {
                                const $this = $(this);
                                let opts;

                                const pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
                                if (pluginOptions)
									opts = pluginOptions;

                                $this.themePluginCarousel(opts);
                            });
						});

						self.$loader.removeClass('portfolio-load-more-loader-showing').hide();

						self.$wrapper.waitForImages(() => {
							self.$wrapper.isotope('layout');
						});

					}, 1000);

				}
			});

		}

	};

    if($('#portfolioLoadMoreWrapper').get(0)) {
		portfolioLoadMore.build();
	}

    /*
	* Ajax on Modal
	*/
    theme.fn.execOnceTroughEvent( 'a[data-ajax-on-modal]', 'mouseover.trigger.ajax.on.modal', () => {
		$('a[data-ajax-on-modal]').magnificPopup({
			type: 'ajax',
			tLoading: '',
			mainClass: 'portfolio-ajax-modal',
			closeBtnInside: true,
			gallery: {
				enabled: true
			},
			callbacks: {
				ajaxContentAdded() {

					// Wrapper
					const $wrapper = $('.portfolio-ajax-modal');

					// Close
					$wrapper.find('a[data-ajax-portfolio-close]').on('click', e => {
						e.preventDefault();
						$.magnificPopup.close();
					});

					// Remove Next and Close
					if($('a[data-ajax-on-modal]').length <= 1) {
						
						$wrapper.find('a[data-ajax-portfolio-prev], a[data-ajax-portfolio-next]').remove();

					} else {

						// Prev
						$wrapper.find('a[data-ajax-portfolio-prev]').on('click', e => {
							e.preventDefault();
							$('.mfp-arrow-left').trigger('click');
							return false;
						});

						// Next
						$wrapper.find('a[data-ajax-portfolio-next]').on('click', e => {
							e.preventDefault();
							$('.mfp-arrow-right').trigger('click');
							return false;
						});

					}

					// Carousel
					$(() => {
						$('[data-plugin-carousel]:not(.manual), .owl-carousel:not(.manual)').each(function() {
                            const $this = $(this);
                            let opts;

                            const pluginOptions = theme.fn.getOptions($this.data('plugin-options'));
                            if (pluginOptions)
								opts = pluginOptions;

                            $this.themePluginCarousel(opts);
                        });
					});

				}
			}
		});
	});

    /*
	* Dialog with CSS animation
	*/
    $('.popup-with-zoom-anim').magnificPopup({
		type: 'inline',

		fixedContentPos: false,
		fixedBgPos: true,

		overflowY: 'auto',

		closeBtnInside: true,
		preloader: false,

		midClick: true,
		removalDelay: 300,
		mainClass: 'my-mfp-zoom-in'
	});
})).apply( this, [ jQuery ]);