(function main() {
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

function generateDetailForOneGame(game) {
    var rootDiv = document.createElement("div");

    var lichessLink = document.createElement("a");
    lichessLink.setAttribute("href", "https://lichess.org/" + game.gameId);
    lichessLink.textContent = "Game on Lichess: " + game.gameId;
    rootDiv.appendChild(lichessLink);

    var sjengPar = document.createElement("p");
    sjengPar.textContent = "Sjeng similarity:";
    rootDiv.appendChild(sjengPar);

    var sjengDiv = document.createElement("div");
    for (var i = 0; i < game.sjengFull.length; i++) {
        var currentMoveComparison = game.sjengFull[i];
        var background = currentMoveComparison === 0 ? "blue" : (currentMoveComparison === 1 ? "red" : "#444444");

        var currentMoveDiv = document.createElement("div");
        currentMoveDiv.style.display = "inline-block";
        currentMoveDiv.style.height = "50px";
        currentMoveDiv.style.width = "20px";
        currentMoveDiv.style.border = "1px solid black";
        currentMoveDiv.style.background = background;

        sjengDiv.appendChild(currentMoveDiv);
    }
    rootDiv.appendChild(sjengDiv);

    var sfPar = document.createElement("p");
    sfPar.textContent = "Stockfish similarity:";
    rootDiv.appendChild(sfPar);

    var sfDiv = document.createElement("div");
    for (var i = 0; i < game.stockfishFull.length; i++) {
        var currentMoveComparison = game.stockfishFull[i];
        var background = currentMoveComparison === 0 ? "blue" : (currentMoveComparison === 1 ? "red" : "#444444");

        var currentMoveDiv = document.createElement("div");
        currentMoveDiv.style.display = "inline-block";
        currentMoveDiv.style.height = "50px";
        currentMoveDiv.style.width = "20px";
        currentMoveDiv.style.border = "1px solid black";
        currentMoveDiv.style.background = background;

        sfDiv.appendChild(currentMoveDiv);
    }
    rootDiv.appendChild(sfDiv);

    return rootDiv;
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
                });

                var sortedGames = _.orderBy(games, [engine], ["desc"]);
                document.getElementById("gameDetails").innerHTML = "";
                for (var i = 0; i < sortedGames.length; i++) {
                    document.getElementById("gameDetails").appendChild(generateDetailForOneGame(sortedGames[i]));
                    document.getElementById("gameDetails").appendChild(document.createElement("hr"));
                }

                if (window.history.pushState) {
                    window.history.pushState({}, null, "/Player/" + lookup + "/" + playerName);
                }
            }
        };
        dataReq.send();
    });

    var pathParts = _.filter(window.location.pathname.split("/"), x => x !== "");
    if (pathParts.length === 3) {
        var lkp = pathParts[1].toLowerCase();
        if (lkp !== "reference" && lkp !== "investigate") {
            window.location.href = "/Player";
        }

        document.getElementById("lookupInput").value = lkp === "reference" ? "Reference" : "Investigate";
        document.getElementById("nameInput").value = pathParts[2];
        document.getElementById("updateBtn").click();
    }
});
})();