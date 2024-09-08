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

