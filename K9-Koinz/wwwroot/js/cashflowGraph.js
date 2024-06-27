function makeCashflowChart() {
    const containerId = "cashflowChartContainer";

    const red = "#C0504E";
    const green = "#9BBB58";
    const blue = "#4F81BC";

    let chart = new CanvasJS.Chart(containerId, {
        backgroundColor: "transparent",
        animationEnabled: true,
        axisX: {
            text: "Month"
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
                type: "column",
                name: "Income",
                showInLegend: true,
                color: green,
                yValueFormatString: "$##,###",
                dataPoints: cashflowIncome
            },
            {
                tyoe: "column",
                name: "Expenses",
                showInLegend: true,
                color: red,
                yValueFormatString: "$##,###",
                dataPoints: cashflowExpenses
            },
        ]
    });

    chart.render();
}