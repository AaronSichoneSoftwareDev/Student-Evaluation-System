window.dashboardCharts = (function () {
    const instances = {};

    function destroy(id) {
        if (instances[id]) {
            instances[id].destroy();
            delete instances[id];
        }
    }

    function renderDonut(id, labels, values, colors) {
        const ctx = document.getElementById(id);
        if (!ctx) return;
        destroy(id);
        instances[id] = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: colors,
                    borderWidth: 0,
                    hoverOffset: 4,
                }],
            },
            options: {
                cutout: '72%',
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
            },
        });
    }

    function renderArea(id, categories, series) {
        const ctx = document.getElementById(id);
        if (!ctx) return;
        destroy(id);
        instances[id] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: categories,
                datasets: series.map(s => ({
                    label: s.name,
                    data: s.values,
                    borderColor: s.color,
                    backgroundColor: s.color + '33',
                    fill: true,
                    tension: 0.4,
                    pointRadius: 0,
                    borderWidth: 2,
                })),
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: { mode: 'index', intersect: false },
                plugins: { legend: { display: false } },
                scales: {
                    x: { grid: { display: false }, ticks: { font: { size: 10 } } },
                    y: { grid: { color: '#eef0f6' }, ticks: { font: { size: 10 } } },
                },
            },
        });
    }

    function renderBar(id, categories, series, showAxes) {
        const ctx = document.getElementById(id);
        if (!ctx) return;
        destroy(id);
        instances[id] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: categories,
                datasets: series.map(s => ({
                    label: s.name,
                    data: s.values,
                    backgroundColor: s.color,
                    borderRadius: 4,
                    maxBarThickness: 18,
                })),
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    x: { grid: { display: false }, ticks: { display: showAxes !== false, font: { size: 10 } } },
                    y: { display: showAxes !== false, grid: { color: '#eef0f6' }, ticks: { font: { size: 10 } } },
                },
            },
        });
    }

    return { renderDonut, renderArea, renderBar, destroy };
})();
