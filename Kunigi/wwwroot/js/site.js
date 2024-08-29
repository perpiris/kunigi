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

document.addEventListener('DOMContentLoaded', function() {
    const teamFilter = document.getElementById('search-filter');
    const teamsContainer = document.getElementById('data-container');
    const teamItems = teamsContainer.getElementsByClassName('data-item');

    teamFilter.addEventListener('input', function() {
        const filterValue = this.value.toLowerCase().trim();
        let visibleCount = 0;

        Array.from(teamItems).forEach(item => {
            const teamName = item.getAttribute('data-item-name');
            if (teamName.includes(filterValue)) {
                item.style.display = '';
                visibleCount++;
            } else {
                item.style.display = 'none';
            }
        });
    });
});

document.getElementById('previous-page-button').addEventListener('click', function(e) {
    e.preventDefault();
    window.history.back();
});