﻿function getCharForTrend(data) {
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

function convertToChartData(data, getSource, getDate, getValue) {
    var series = {};

    $.each(data, function (key, val) {
        var source = getSource(val);
        var items;
        if (!series[source]) {
            series[source] = {
                name: source,
                data: []
            };
        }
        items = series[source];
        items.data.push([getDate(val), getValue(val)]);
    });

    var data = [];
    for (var s in series) {
        data.push(series[s]);
    }

    return data;
}

function getChartDataFromTemperatures(data) {
    return convertToChartData(data, 
        function (val) { return val.Source }, 
        function (val) { var d = new Date(val.MeasurementDateTimeUtc); return Date.UTC(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds()) },
        function (val) { return val.TemperatureFahrenheit }
        );
}

function getChartDataFromHumidities(data) {
    return convertToChartData(data,
        function (val) { return val.Source },
        function (val) { var d = new Date(val.MeasurementDateTimeUtc); return Date.UTC(d.getFullYear(), d.getMonth(), d.getDate(), d.getHours(), d.getMinutes(), d.getSeconds()) },
        function (val) { return val.HumidityPercentage }
        );
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
}

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
    })).done(function(){
        var temperatureData = getChartDataFromTemperatures(temperatures);
        var humidityData = getChartDataFromHumidities(humidities);
        renderChart('container', temperatureData.concat(humidityData));
    });
}