function fillTemperatureChart() {
    // make two async calls to get temperatures and humidities
    // when each call finishes, store the result in a local variable
    // when both calls have finished, parse the results and then send them to
    // the chart rendering function
    var temperatures = [];
    var humidities = [];
    $.when($.getJSON("api/temperature/", function (data) {
        temperatures = data;
    }), $.getJSON("api/humidity/", function (data) {
        humidities = data;
    })).done(function () {
        var temperatureData = convertToChartData(temperatures, { sourcePropertyName: "Source", utcDatePropertyName: "MeasurementDateTimeUtc", valuePropertyName: "TemperatureFahrenheit" });
        var humidityData = convertToChartData(humidities, { sourcePropertyName: "Source", utcDatePropertyName: "MeasurementDateTimeUtc", valuePropertyName: "HumidityPercentage", seriesSuffix: " (Humidity)" });
        renderChart('container', temperatureData.concat(humidityData));
    });
}

function loadLatestTemperatures() {
    var temperatureSources = [];
    var humiditySources = [];

    $.when(
        $.getJSON("api/temperature/latest", function (data) {
            $.each(data, function (key, val) {
                var sourceName = val.SourceName;
                var temperature = val.TemperatureFahrenheit + "°F";
                var trend = getCharForTrend(val.Trend);

                temperatureSources.push(sourceName + ": " + temperature + " " + trend);
            });
        }),
        $.getJSON("api/humidity/latest", function (data) {
            $.each(data, function (key, val) {
                var sourceName = val.SourceName + " (Humidity)";
                var humidity = val.HumidityPercentage + "%";
                var trend = getCharForTrend(val.Trend);

                humiditySources.push(sourceName + ": " + humidity + " " + trend);
            });
        }))
    .done(function () {
        $.each(temperatureSources.concat(humiditySources), function(key, val){
            $('<li/>', { "text": val, "class": "list-group-item latest-reading" })
                .appendTo($('#latest_temperatures'));
        });
    });
}

function getCharForTrend(data) {
    var trend = '?';
    switch (data) {
        case 0:
            trend = '↑';
            break;
        case 1:
            trend = '↓';
            break;
        case 2:
            trend = '↔';
            break;
    }

    return trend;
}

function convertToChartData(data, options) {
    var series = {};
    var sourcePropertyName = options.sourcePropertyName;
    var utcDatePropertyName = options.utcDatePropertyName;
    var valuePropertyName = options.valuePropertyName;
    var seriesSuffix = options.seriesSuffix || "";

    $.each(data, function (key, val) {
        var source = val[sourcePropertyName] + seriesSuffix;
        var items;
        if (!series[source]) {
            series[source] = {
                name: source,
                data: []
            };
        }
        items = series[source];
        var d = new Date(val[utcDatePropertyName]);
        var utc = Date.UTC(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds());
        items.data.push([utc, val[valuePropertyName]]);
    });

    var data = [];
    for (var s in series) {
        data.push(series[s]);
    }

    return data;
}

function renderChart(containerId, data){
    var chart = new Highcharts.Chart({
        chart: {
            renderTo: containerId,
            type: 'spline',
            zoomType: 'x'
        },
        title: {
            text: 'Last 24 Hours'
        },
        xAxis: {
            type: 'datetime',
            dateTimeLabelFormats: {
                millisecond: '%I:%M:%S.%L %p',
                second: '%I:%M:%S %p',
                minute: '%I:%M %p',
                hour: '%I:%M %p'
            },
            tickPixelInterval: 200,
        },
        yAxis: [
            { title: { text: 'Fahrenheit' } },
            { title: { text: '%' }, opposite: true },
        ],
        tooltip: {
            formatter: function () {
                return this.series.name + '<br/>' + Highcharts.dateFormat("%l:%M %p", this.x, true) + '<br/>' + this.y + (this.series.name.indexOf("Humidity") == -1 ? '°F' : '%');
            }
        },
        plotOptions: {
            spline: {
                lineWidth: 4,
                states: {
                    hover: {
                        lineWidth: 5
                    }
                },
                marker: {
                    enabled: false
                }
            }
        },
        legend: {
            enabled: true
        },
        series: data
    });
}