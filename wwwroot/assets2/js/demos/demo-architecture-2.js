/*
Name: 			Architecture 2
Written by: 	Okler Themes - (http://www.okler.net)
Theme Version:	12.1.0
*/

(($ => {
    /*
	* Slider Background
	*/
    const $slider = $('#slider');

    let direction = '';

    $slider.on('click', '.owl-next', () => {
		direction = 'next';
	});

    $slider.on('click', '.owl-prev', () => {
		direction = 'prev';
	});

    $slider.on('changed.owl.carousel', ({item}) => {
		
		$('.custom-slider-background .custom-slider-background-image-stage').each(function(){
			const $stage       = $(this), $stageOuter  = $stage.closest('.custom-slider-background-image-stage-outer'), $currentItem = $stage.find('.custom-slider-background-image-item').eq( item.index ), nItems       = $stage.find('.custom-slider-background-image-item').length;

			const distance = $stage.hasClass('reverse') ? ( $currentItem.outerHeight() * nItems ) - ( $currentItem.outerHeight() * ( item.index + 1 ) ) : $currentItem.outerHeight() * item.index, mathSymbol = $stage.hasClass('reverse') ? '-' : '-'; 

			$stage.css({
				transform: 'translate3d(0, '+ mathSymbol + distance +'px, 0)'
			});
		});

	});

    // Once we have all ready, show the slider
    $slider.on('initialized.owl.carousel', () => {
		setTimeout(() => {
			$('.custom-slider-background').addClass('show');
		}, 800);
	});

    // Hide nav on first load of page
    $slider.on('initialized.owl.carousel', () => {
		setTimeout(() => {
			$slider.find('.owl-nav').addClass('hide');
		}, 200);
	});

    // Show nav once the slider animation is completed
    $('.custom-slider-background').parent().on('transitionend', () => {
		setTimeout(() => {
			$slider.find('.owl-nav').addClass('show');
			$('.custom-slider-background').addClass('custom-box-shadow-1');
		}, 2000);
	});

    /*
	* Page Header
	*/
    $('.custom-page-header-1-wrapper > div').on('animationend', () => {
		setTimeout(() => {
			$('.custom-page-header-1-wrapper').addClass('custom-box-shadow-1');
		}, 1000);
	});

    /*
	* Load More - Projects
	*/
    const loadMore = {

		pages: 0,
		currentPage: 1,
		$wrapper: $('#loadMoreWrapper'),
		$btn: $('#loadMore'),
		$btnWrapper: $('#loadMoreBtnWrapper'),
		$loader: $('#loadMoreLoader'),

		build() {

			const self = this;

			self.pages = self.$wrapper.data('total-pages');

			if(self.pages <= 1) {

				self.$btnWrapper.remove();
				return;

			} else {

				// init isotope
				self.$wrapper.isotope();

				self.$btn.on('click', () => {
					self.loadMore();
				});

				// Lazy Load
				if(self.$btn.hasClass('btn-lazy-load')) {
					theme.fn.intObs( '#loadMore', "$('#loadMore').trigger('click');", {
						rootMargin: '0px 0px 0px 0px'
					} );
				}

			}

		},
		loadMore() {

			const self = this;

			self.$btn.css({
				opacity: 0
			});
			self.$loader.show();

			// Ajax
			$.ajax({
				url: 'ajax/demo-architecture-2-ajax-projects-load-more-' + (parseInt(self.currentPage)+1) + '.html',
				complete({responseText}) {

					const $items = $(responseText);

					setTimeout(() => {

						self.$wrapper.append($items)

						self.$wrapper.isotope('appended', $items);

						self.currentPage++;

						if(self.currentPage < self.pages) {
							self.$btn.css({
								opacity: 1
							}).blur();
						} else {
							self.$btnWrapper.remove();
						}

						self.$loader.hide();

					}, 1000);

				}
			});

		}

	};

    if($('#loadMoreWrapper').get(0)) {
		loadMore.build();
	}
})).apply( this, [ jQuery ]);