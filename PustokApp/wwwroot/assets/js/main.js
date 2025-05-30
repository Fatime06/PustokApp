﻿$(document).ready(function () {
    $(".book-modal").click(function (e) {
        e.preventDefault();
        let url = ($(this).attr("href"));
        fetch(url)
            .then(response => response.text())
            .then(data => {
                $("#quickModal ,modal-dialog").html(data);
                const mainSliderSettings = {
                    "slidesToShow": 1,
                    "arrows": false,
                    "fade": true,
                    "draggable": false,
                    "swipe": false,
                    "asNavFor": ".product-slider-nav"
                };

                const navSliderSettings = {
                    "infinite": true,
                    "autoplay": true,
                    "autoplaySpeed": 8000,
                    "slidesToShow": 4,
                    "arrows": true,
                    "prevArrow": { "buttonClass": "slick-prev", "iconClass": "fa fa-chevron-left" },
                    "nextArrow": { "buttonClass": "slick-next", "iconClass": "fa fa-chevron-right" },
                    "asNavFor": ".product-details-slider",
                    "focusOnSelect": true
                };

                const mainSliderElement = document.querySelector('.product-details-slider');
                if (mainSliderElement) {
                    $(mainSliderElement).click(mainSliderSettings);
                }

                const navSliderElement = document.querySelector('.product-slider-nav');
                if (navSliderElement) {
                    $(navSliderElement).click(navSliderSettings);
                }

            })
        $("#quickModal").modal("show");

    })

    
})