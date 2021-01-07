//<script src="https://code.highcharts.com/highcharts.js"></script>
//<script src="https://code.highcharts.com/modules/exporting.js"></script>
//<script src="https://code.highcharts.com/modules/export-data.js"></script>
//<script src="https://code.highcharts.com/modules/accessibility.js"></script>

Highcharts.chart('container', {
    chart: {
        type: 'column'
    },
    title: {
        text: 'Evaluating website.com performance'
    },
    xAxis: {
        categories: [
            '1',
            '2',
            '3',
            '4',
            '5'
        ],
        crosshair: true
    },
    yAxis: {
        min: 0,
        title: {
            text: 'Evaluating (seconds)'
        }
    },
    tooltip: {
        headerFormat: '<span style="font-size:10px">{point.key} attempt</span><table>',
        pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
            '<td style="padding:0"><b>{point.y:.1f} s</b></td></tr>',
        footerFormat: '</table>',
        shared: true,
        useHTML: true
    },
    plotOptions: {
        column: {
            pointPadding: 0.2,
            borderWidth: 0
        }
    },
    series: [{
        name: 'website.com',
        data: [0.423, 0.301, 0.435, 0.421, 0.378]

    }, {
        name: 'website.com/news',
        data: [0.123, 0.201, 0.135, 0.221, 0.178]

    }, {
        name: 'website.com/about',
        data: [0.423, 0.401, 0.335, 0.621, 0.678]

    }, {
        name: 'website.com/contacts',
        data: [0.223, 0.323, 0.345, 0.521, 0.478]

    }]
});


//.highcharts - figure, .highcharts - data - table table {
//    min - width: 310px;
//    max - width: 1170px;
//    margin: 1em auto;
//}

//#container {
//    height: 400px;
//    max - width: 1170px;
//}

//.highcharts - data - table table {
//    font - family: Verdana, sans - serif;
//    border - collapse: collapse;
//    border: 1px solid #EBEBEB;
//    margin: 10px auto;
//    text - align: center;
//    width: 100 %;
//    max - width: 1170px;
//}

//.highcharts - data - table caption {
//    padding: 1em 0;
//    font - size: 1.2em;
//    color: #555;
//}

//.highcharts - data - table th {
//    font - weight: 600;
//    padding: 0.5em;
//}

//.highcharts - data - table td, .highcharts - data - table th, .highcharts - data - table caption {
//    padding: 0.5em;
//}

//.highcharts - data - table thead tr, .highcharts - data - table tr: nth - child(even) {
//    background: #f8f8f8;
//}

//.highcharts - data - table tr: hover {
//    background: #f1f7ff;
//}