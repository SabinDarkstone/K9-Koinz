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
                dataPoints: [
                    { label: "Dec '23", y: 5674 },
                    { label: "Jan '24", y: 7064 },
                    { label: "Feb '24", y: 8435 },
                    { label: "Mar '24", y: 8587 },
                    { label: "Apr '24", y: 6746 },
                    { label: "May '24", y: 0 }
                ]
            },
            {
                tyoe: "column",
                name: "Expenses",
                showInLegend: true,
                color: red,
                yValueFormatString: "$##,###",
                dataPoints: [
                    { label: "Dec '23", y: 2517 + 2076 + 2460 },
                    { label: "Jan '24", y: 1939 + 568 + 2832 },
                    { label: "Feb '24", y: 1160 + 1815 + 2476 },
                    { label: "Mar '24", y: 1881 + 958 + 2535 },
                    { label: "Apr '24", y: 2158 + 716 + 2457 },
                    { label: "May '24", y: 304 + 24 + 2694 }
                ]
            },
            {
                type: "column",
                name: "Savings Goals",
                showInLegend: true,
                color: blue,
                yValueFormatString: "$##,###",
                dataPoints: [
                    { label: "Dec '23", y: 0 },
                    { label: "Jan '24", y: 0 },
                    { label: "Feb '24", y: 0 },
                    { label: "Mar '24", y: 638 },
                    { label: "Apr '24", y: 1559 },
                    { label: "May '24", y: 1739 }
                ]
            }
        ]
    });

    chart.render();
}