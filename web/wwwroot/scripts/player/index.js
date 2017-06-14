var currentChart = null;

function generateLabels(step) {
    var count = 100 / step + 1;
    var labels = [];
    for (var i = 0; i < count; i++) {
        labels.push(i * step);
    }
    return labels;
}

function distributionObjectToList(step, dist) {
    var list = [];
    for (var i = 0; i <= 100; i += step) {
        if (Object.keys(dist).indexOf(i.toString()) === -1) {
            list.push(0);
        } else {
            list.push(dist[i]);
        }
    }
    return list;
}

window.addEventListener("load", function() {
    document.getElementById("updateBtn").addEventListener("click", function(e) {
        e.preventDefault();

        var playerName = document.getElementById("nameInput").value;
        var lookup = document.getElementById("lookupInput").value;
        var url = "/Player/{lookup}/{name}/Games?minRating=0&maxRating=3000&speed=all".replace("{lookup}", lookup).replace("{name}", playerName);

        var step = 5;
        var engine = "max";

        var dataReq = new XMLHttpRequest();
        dataReq.open("GET", url);
        dataReq.onreadystatechange = function() {
            if (dataReq.readyState == XMLHttpRequest.DONE && dataReq.status === 200) {
                if (currentChart) {
                    currentChart.destroy();
                }

                var games = JSON.parse(dataReq.responseText);
                var distribution = _.countBy(games, x => Math.floor(x[engine] * 100 / step) * step);
                var distList = distributionObjectToList(step, distribution);

                var ctx = document.getElementById("distributionCanvas");
                currentChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: generateLabels(step),
                        datasets: [{
                            label: 'Distribution',
                            data: distList
                        }]
                    },
                    options: { responsive: false }
                })
            }
        };
        dataReq.send();
    });
});