function chartLoad(nameValues, minValues, maxValues) {
    var trace1 = {
        x: nameValues,
        y: minValues,
        name: 'Min response',
        marker: { color: 'rgb(121, 217, 161)' },
        type: 'bar'
    };
    var trace2 = {
        x: nameValues,
        y: maxValues,
        name: 'Max response',
        marker: { color: 'rgb(55, 83, 109)' },
        type: 'bar'
    };
    var data = [trace1, trace2];
    var layout = {
        title: '',
        xaxis: {
            tickfont: {
                size: 1,
                color: 'rgb(255, 255, 255)'
            }
        },
        yaxis: {
            title: 'Response time (ms)',
            titlefont: {
                size: 16,
                color: 'rgb(107, 107, 107)'
            },
            tickfont: {
                size: 14,
                color: 'rgb(107, 107, 107)'
            }
        },
        legend: {
            x: 1,
            y: 1.0,
            bgcolor: 'rgba(255, 255, 255, 0)',
            bordercolor: 'rgba(255, 255, 255, 0)'
        },
        barmode: 'group',
        bargap: 0.15,
        bargroupgap: 0.1
    };
    Plotly.newPlot('myDiv', data, layout);
}
//bottom legend x: 0.425, y: -0.2