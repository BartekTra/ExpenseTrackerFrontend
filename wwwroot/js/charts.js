let charts = {};

window.createChart = function (canvasId, config) {
    const canvas = document.getElementById(canvasId);
    if (charts[canvasId]) {
        charts[canvasId].destroy();
    }
    charts[canvasId] = new Chart(canvas, config);
    return charts[canvasId];
};

window.destroyChart = function (canvasId) {
    if (charts[canvasId]) {
        charts[canvasId].destroy();
        delete charts[canvasId];
    }
}; 