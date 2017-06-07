(function main() {

var charts = { "legit": null, "cheat": null };
var chartIds = { "legit": "legit-distribution", "cheat": "cheat-distribution" };

function updateChartsBtnClicked(e) {
    e.preventDefault();

    updateCharts();
}

function updateCharts() {
    updateOne("legit");
    updateOne("cheat");
}

function updateOne(cl) {
    var engine = document.getElementById("compareEngineDropdown").value;
    var minRating = document.getElementById("minRatingDropdown").value;
    var maxRating = document.getElementById("maxRatingDropdown").value;
    var speed = document.getElementById("speedDropdown").value;
    var step = document.getElementById("stepInput").value;

    var url = "/Reference/Select?cheatOrLegit={0}&engine={1}&minRating={2}&maxRating={3}&speed={4}&step={5}".replace("{0}", cl)
                                                                                                            .replace("{1}", engine)
                                                                                                            .replace("{2}", minRating)
                                                                                                            .replace("{3}", maxRating)
                                                                                                            .replace("{4}", speed)
                                                                                                            .replace("{5}", step);
    var dataReq = new XMLHttpRequest();
    dataReq.open("GET", url);
    dataReq.onreadystatechange = function() {
        if (dataReq.readyState == XMLHttpRequest.DONE && dataReq.status === 200) {
            if (charts[cl]) {
                charts[cl].destroy();
            }

            var ctx = document.getElementById(chartIds[cl]);
            charts[cl] = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: new Array(100 / parseFloat(step) + 1),
                    datasets: [{
                        label: 'Distribution',
                        data: dataReq.responseText.split(',').map(Number)
                    }]
                },
                options: { responsive: false }
            })
        }
    };
    dataReq.send();
}

window.addEventListener("load", function() {
    document.getElementById("updateBtn").addEventListener("click", updateChartsBtnClicked);
});

})();