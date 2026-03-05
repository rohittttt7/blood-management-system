// Blood Bank Management System - Enhanced UI/UX JavaScript
// Modern Interactive Features

$(document).ready(function() {

    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert:not(.alert-permanent)').fadeOut('slow', function() {
            $(this).remove();
        });
    }, 5000);

    // Add animation on scroll
    function animateOnScroll() {
        $('.animate-on-scroll').each(function() {
            var elementTop = $(this).offset().top;
            var elementBottom = elementTop + $(this).outerHeight();
            var viewportTop = $(window).scrollTop();
            var viewportBottom = viewportTop + $(window).height();

            if (elementBottom > viewportTop && elementTop < viewportBottom) {
                $(this).addClass('animate-fadeInUp');
            }
        });
    }

    $(window).on('scroll', animateOnScroll);
    animateOnScroll();

    // Loading animation for forms
    $('form').on('submit', function() {
        var btn = $(this).find('button[type="submit"]');
        if (btn.length && !btn.hasClass('no-loading')) {
            var originalText = btn.html();
            btn.html('<i class="fas fa-spinner fa-spin me-2"></i>Processing...');
            btn.prop('disabled', true);
        }
    });

    // Initialize tooltips
    if (typeof bootstrap !== 'undefined') {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // Sticky navigation on scroll
    $(window).scroll(function() {
        if ($(this).scrollTop() > 50) {
            $('.navbar').addClass('shadow-lg');
        } else {
            $('.navbar').removeClass('shadow-lg');
        }
    });

    // Blood drop animation
    setInterval(function() {
        $('.blood-drop').toggleClass('animate-pulse');
    }, 3000);

    // Number counter animation for statistics
    $('.counter').each(function() {
        var $this = $(this);
        var countTo = parseInt($this.text().replace(/,/g, ''));

        if (!isNaN(countTo)) {
            $({ countNum: 0 }).animate({
                countNum: countTo
            }, {
                duration: 2000,
                easing: 'swing',
                step: function() {
                    $this.text(Math.floor(this.countNum).toLocaleString());
                },
                complete: function() {
                    $this.text(countTo.toLocaleString());
                }
            });
        }
    });

    // Real-time form validation feedback
    $('input.form-control, select.form-select, textarea.form-control').on('blur', function() {
        if ($(this).prop('required') && !$(this).val()) {
            $(this).addClass('is-invalid');
            $(this).removeClass('is-valid');
        } else if ($(this).val()) {
            $(this).removeClass('is-invalid');
            $(this).addClass('is-valid');
        }
    });

    // Emergency checkbox highlight
    $('input[name="isEmergency"]').on('change', function() {
        if ($(this).is(':checked')) {
            $(this).closest('.form-check').addClass('border border-danger border-3 p-3 rounded bg-danger bg-opacity-10');
        } else {
            $(this).closest('.form-check').removeClass('border border-danger border-3 p-3 rounded bg-danger bg-opacity-10');
        }
    });

    // Show/hide password toggle
    $('.toggle-password').on('click', function() {
        var input = $($(this).attr('data-target'));
        var icon = $(this).find('i');

        if (input.attr('type') === 'password') {
            input.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            input.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    // Print button
    $('.btn-print').on('click', function(e) {
        e.preventDefault();
        window.print();
    });

    // Smooth scroll for anchor links
    $('a[href^="#"]').on('click', function(event) {
        var target = $(this.getAttribute('href'));
        if(target.length) {
            event.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 80
            }, 1000);
        }
    });

    console.log('Blood Bank Management System - UI Enhanced ✓');
});

