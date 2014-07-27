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

function loadLatestTemperatures() {
    $.getJSON("api/temperature/latest",
                function (data) {
                    // On success, 'data' contains a list of LatestTemperatureInfos
                    $.each(data, function (key, val) {
                        var sourceName = val.SourceName;
                        var temperature = val.TemperatureFahrenheit + "°F";
                        var trend = getCharForTrend(val.Trend);

                        $('<li/>', { text: sourceName + ": " + temperature + " " + trend })
                        .appendTo($('#latest_temperatures'));
                    });

                    // Send an AJAX request
                    $.getJSON("api/humidity/latest",
                    function (data) {
                        // On success, 'data' contains a list of LatestTemperatureInfos
                        $.each(data, function (key, val) {
                            var sourceName = val.SourceName + " (Humidity)";
                            var humidity = val.HumidityPercentage + "%";
                            var trend = getCharForTrend(val.Trend);

                            $('<li/>', { text: sourceName + ": " + humidity + " " + trend })
                            .appendTo($('#latest_temperatures'));
                        });
                    });
                });
}

function fillTemperatureChart() {
    $.getJSON("api/temperature/",
                    function (data) {
                        var series = {};

                        $.each(data, function (key, val) {
                            var source = val.Source;
                            var items;
                            if (!series[source]) {
                                series[source] = {
                                    name: source,
                                    data: []
                                };
                            }
                            items = series[source];
                            var d = new Date(val.MeasurementDateTimeUtc);
                            items.data.push([Date.UTC(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds()), val.TemperatureFahrenheit]);
                        });

                        var data = [];
                        for (var s in series) {
                            data.push(series[s]);
                        }

                        var chart = new Highcharts.Chart({
                            chart: {
                                renderTo: 'container',
                                type: 'spline',
                                zoomType: 'x'
                            },
                            title: {
                                text: 'Last 24 Hours'
                            },
                            xAxis: {
                                type: 'datetime',
                                dateTimeLabelFormats: {
                                    hour: '%I:%M %p'
                                },
                                maxZoom: 24 * 3600000 // 1 day
                            },
                            yAxis: [
                                { title: { text: 'Fahrenheit' } },
                                { title: { text: '%' }, opposite: true },
                            ],
                            tooltip: {
                                formatter: function () {
                                    return this.series.name + '<br/>' + Highcharts.dateFormat("%l:%M %p", this.x, true) + '<br/>' + this.y + '°F';
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

                        $.getJSON("api/humidity/",
                            function (data) {
                                var humiditySeries = {};

                                $.each(data, function (key, val) {
                                    var source = val.Source + " (Humidity)";
                                    var items;
                                    if (!humiditySeries[source]) {
                                        humiditySeries[source] = {
                                            name: source,
                                            data: [],
                                            yAxis: 1,
                                        };
                                    }
                                    items = humiditySeries[source];
                                    var d = new Date(val.MeasurementDateTimeUtc);
                                    items.data.push([Date.UTC(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds()), val.HumidityPercentage]);
                                });

                                var humidityData = [];
                                for (var s in humiditySeries) {
                                    chart.addSeries(humiditySeries[s]);
                                }
                            });
                    });
}