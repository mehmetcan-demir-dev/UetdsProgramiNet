/*
Name: 			Architecture & Interior Design
Written by: 	Okler Themes - (http://www.okler.net)
Theme Version:	12.1.0
*/

(($ => {
    // Custom Content Rotator
    $('.custom-content-rotator').each(function(){
		$(this).textRotator({
			fadeSpeed: 500,
			pauseSpeed: 5000
		});
	});

    // Load More - Projects
    var portfolioLoadMore = {

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
					} );
				}

			}

		},
		loadMore() {

			const self = this, ajax_url = ( self.$wrapper.data('ajax-url') ) ? self.$wrapper.data('ajax-url') : 'ajax/portfolio-ajax-load-more-';

			self.$btn.parent().find('.btn').hide();
			self.$loader.addClass('portfolio-load-more-loader-showing').show();

			// Ajax
			$.ajax({
				url: ajax_url + (parseInt(self.currentPage)+1) + '.html',
				complete({responseText}) {

					const $items = $(responseText);

					setTimeout(() => {

						self.$wrapper.append($items);

						self.currentPage++;

						if(self.currentPage < self.pages) {
							self.$btn.parent().find('.btn').show().blur();
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

					}, 1000);

				}
			});

		}

	}

    if($('#portfolioLoadMoreWrapper').get(0)) {
		portfolioLoadMore.build();
	}

    // Load More - Blog
    var portfolioLoadMore = {

		pages: 0,
		currentPage: 1,
		$wrapper: $('#portfolioLoadMoreWrapperBlog'),
		$btn: $('#portfolioLoadMoreBlog'),
		$btnWrapper: $('#portfolioLoadMoreBtnWrapperBlog'),
		$loader: $('#portfolioLoadMoreLoaderBlog'),

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
					theme.fn.intObs( '#portfolioLoadMoreBlog', "$('#portfolioLoadMoreBlog').trigger('click');", {
						rootMargin: '0px 0px 0px 0px'
					} );
				}

			}

		},
		loadMore() {

			const self = this, ajax_url = ( self.$wrapper.data('ajax-url') ) ? self.$wrapper.data('ajax-url') : 'ajax/portfolio-ajax-load-more-';

			self.$btn.parent().find('.btn').hide();
			self.$loader.addClass('portfolio-load-more-loader-showing').show();

			// Ajax
			$.ajax({
				url: ajax_url + (parseInt(self.currentPage)+1) + '.html',
				complete({responseText}) {

					const $items = $(responseText);

					setTimeout(() => {

						self.$wrapper.append($items);

						self.currentPage++;

						if(self.currentPage < self.pages) {
							self.$btn.parent().find('.btn').show().blur();
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

					}, 1000);

				}
			});

		}

	}

    if($('#portfolioLoadMoreWrapperBlog').get(0)) {
		portfolioLoadMore.build();
	}

    /*
	Services Ajax
	*/
    $('.simple-ajax-popup').magnificPopup({
		type: 'ajax',
		callbacks: {
			open() {
				$('html').addClass('lightbox-opened');
			},
			close() {
				$('html').removeClass('lightbox-opened');
			}
		}
	});
})).apply( this, [ jQuery ]);