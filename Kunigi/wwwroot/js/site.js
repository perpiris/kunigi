document.addEventListener('DOMContentLoaded', function() {
    const filterInput = document.getElementById('filterInput');
    const managerSelect = document.getElementById('managerSelect');

    filterInput.addEventListener('input', function() {
        const filterValue = filterInput.value.toLowerCase();
        const options = Array.from(managerSelect.options);

        options.forEach(option => {
            const optionText = option.text.toLowerCase();
            option.style.display = optionText.includes(filterValue) ? '' : 'none';
        });
    });
});