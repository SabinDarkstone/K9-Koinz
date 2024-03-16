window.onload = () => {
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
        data: [
            {
                type: "spline",
                name: "This Month",
                dataPoints: thisMonth,
                markerType: "none",
                lineThickness: 5,
                showInLegend: true,
                yValueFormatString: "$##,###"
            },
            {
                type: "spline",
                name: "Last Month",
                dataPoints: lastMonth,
                markerType: "none",
                lineThickness: 5,
                showInLegend: true,
                yValueFormatString: "$##,###"
            }
        ]
    });
    chart.render();

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