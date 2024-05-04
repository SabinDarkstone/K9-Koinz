window.onload = () => {
    const red = "#C0504E";
    const green = "#9BBB58";
    const blue = "#4F81BC";
    var chart = new CanvasJS.Chart("spendingChartContainer", {
        backgroundColor: "transparent",
        animationEnabled: false,
        axisX: {
            text: "Day"
        },
        axisY: {
            title: "Dollars",
            prefix: "$"
        },
        legend: {
            cursor: "pointer",
            itemclick: "toggleDataSeries"
        },
        toolTip: {
            shared: true
        },
        data: [
            {
                type: "line",
                name: "Last Month",
                dataPoints: lastMonth,
                markerType: "none",
                lineThickness: 5,
                showInLegend: true,
                yValueFormatString: "$##,###",
                lineDashType: "solid",
                color: red
            },
            {
                type: "line",
                name: "3 Month Average",
                dataPoints: threeMonthAverage,
                markerType: "none",
                lineThickness: 3,
                showInLegend: true,
                yValueFormatString: "$##,###",
                lineDashType: "dash",
                color: green
            },
            {
                type: "line",
                name: "This Month",
                dataPoints: thisMonth,
                markerType: "none",
                lineThickness: 5,
                showInLegend: true,
                yValueFormatString: "$##,###",
                lineDashType: "solid",
                color: blue
            },
        ]
    });
    chart.render();

    function toggleDataSeries(e) {
        if (typeof (e.dataSeries.visible) === "undefined" || e.dataSeries.visible) {
            e.dataSeries.visible = false;
        } else {
            e.dataSeries.visible = true;
        }
        chart.render();
    }

    // var chart2 = new CanvasJS.Chart("spendingChartContainer2", {
    //     animationEnabled: false,
    //     title: {
    //         text: "Monthly Spending"
    //     },
    //     axisX: {
    //         text: "Day"
    //     },
    //     axisY: {
    //         title: "Dollars",
    //         prefix: "$"
    //     },
    //     data: [
    //         {
    //             type: "spline",
    //             name: "This Month",
    //             dataPoints: thisMonth,
    //             markerType: "none",
    //             lineThickness: 5,
    //             showInLegend: true,
    //             yValueFormatString: "$##,###"
    //         },
    //         {
    //             type: "spline",
    //             name: "Last Month",
    //             dataPoints: lastMonth,
    //             markerType: "none",
    //             lineThickness: 5,
    //             showInLegend: true,
    //             yValueFormatString: "$##,###"
    //         }
    //     ]
    // });
    // chart2.render();
};