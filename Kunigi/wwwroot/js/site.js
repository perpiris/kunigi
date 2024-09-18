$(document).ready(function() {

    function setupAlerts() {
        $('.alert.auto-dismiss').each(function() {
            const $alert = $(this);
            let dismissTimeout;

            function startDismissTimer() {
                dismissTimeout = setTimeout(function() {
                    dismissAlert($alert);
                }, 5000);
            }

            function stopDismissTimer() {
                clearTimeout(dismissTimeout);
            }

            startDismissTimer();

            $alert.hover(stopDismissTimer, startDismissTimer);
        });
    }

    function dismissAlert($alert) {
        if (typeof bootstrap !== 'undefined') {
            const bsAlert = new bootstrap.Alert($alert[0]);
            bsAlert.close();
        } else {
            $alert.fadeOut(500, function() {
                $(this).remove();
            });
        }
    }
    
    function applyValidationClasses() {
        $('input, select, textarea').each(function() {
            const $input = $(this);
            const $errorSpan = $input.next('.field-validation-error');

            if ($errorSpan.length > 0 && $errorSpan.text().trim() !== '') {
                $input.addClass('is-invalid');
            } else {
                $input.removeClass('is-invalid');
            }
        });
    }

    function setupFormValidation() {
        const $form = $('form');

        $form.on('submit', function() {
            setTimeout(applyValidationClasses, 0);
        });

        $form.on('change keyup', 'input, select, textarea', function() {
            const $input = $(this);
            const $errorSpan = $input.next('.field-validation-error');

            if ($errorSpan.length === 0 || $errorSpan.text().trim() === '') {
                $input.removeClass('is-invalid');
            }
        });

        $form.on('blur', 'input, select, textarea', function() {
            const $input = $(this);
            $.validator.unobtrusive.parseElement(this);
            $input.valid();
            applyValidationClasses();
        });
    }
    
    setupAlerts();
    applyValidationClasses();
    setupFormValidation();
});