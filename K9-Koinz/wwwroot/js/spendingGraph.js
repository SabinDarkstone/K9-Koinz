function makeSpendingChart() {
    const containerId = "spendingChartContainer";

    const red = "#C0504E";
    const green = "#9BBB58";
    const blue = "#4F81BC";

    let chart = new CanvasJS.Chart(containerId, {
        backgroundColor: "transparent",
        animationEnabled: true,
        axisX: {
            text: "Day of Month"
        },
        axisY: {
            title: "Dollars",
            prefix: "$"
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
}