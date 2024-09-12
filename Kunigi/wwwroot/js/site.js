document.addEventListener('DOMContentLoaded', function() {
    const alerts = document.querySelectorAll('.alert.auto-dismiss');

    alerts.forEach(function(alert) {
        setTimeout(function() {
            dismissAlert(alert);
        }, 3000);
        
        alert.addEventListener('mouseenter', function() {
            clearTimeout(alert.dismissTimeout);
        });

        alert.addEventListener('mouseleave', function() {
            alert.dismissTimeout = setTimeout(function() {
                dismissAlert(alert);
            }, 3000);
        });
    });

    function dismissAlert(alert) {
        if (typeof bootstrap !== 'undefined') {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        } else {
            alert.style.opacity = '0';
            setTimeout(function() {
                alert.remove();
            }, 300);
        }
    }
});

$(document).ready(function () {
    function applyValidationClasses() {
        $('input, select, textarea').each(function () {
            let inputElement = $(this);
            let errorSpan = inputElement.next('.field-validation-error');

            if (errorSpan.length > 0 && errorSpan.text().trim() !== '') {
                inputElement.addClass('is-invalid');
            } else {
                inputElement.removeClass('is-invalid');
            }
        });
    }

    applyValidationClasses();

    $('form').submit(function () {
        setTimeout(applyValidationClasses, 0);
    });

    // Remove is-invalid class on input change
    $('form').on('change keyup', 'input, select, textarea', function() {
        let inputElement = $(this);
        let errorSpan = inputElement.next('.field-validation-error');

        if (errorSpan.length === 0 || errorSpan.text().trim() === '') {
            inputElement.removeClass('is-invalid');
        }
    });

    // Re-validate on blur
    $('form').on('blur', 'input, select, textarea', function() {
        let form = $(this).closest('form');
        $.validator.unobtrusive.parseElement(this);
        $(this).valid();
        applyValidationClasses();
    });
});

