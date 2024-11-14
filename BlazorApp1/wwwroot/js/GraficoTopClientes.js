function GeraGraficoTopClientes(GetTopClientes, GetTopClientesMaisCompras) {
       
    // Create root element
    // https://www.amcharts.com/docs/v5/getting-started/#Root_element
    var root = am5.Root.new("chartdiv1");

    // Set themes
    // https://www.amcharts.com/docs/v5/concepts/themes/
    root.setThemes([
        am5themes_Animated.new(root)
    ]);

    root.container.set("layout", root.verticalLayout);

    // Create container to hold charts
    var chartContainer = root.container.children.push(am5.Container.new(root, {
        layout: root.horizontalLayout,
        width: am5.p100,
        height: am5.p100
    }));

    // Create the 1st chart
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/
    //var chart = root.container.children.push(
    var chart = chartContainer.children.push(
        am5percent.PieChart.new(root, {
            endAngle: 270,
            innerRadius: am5.percent(60)
        })
    );

    // Set data
    var data = GetTopClientes.map(cliente => ({
        value: cliente.totalComprado, // Altere conforme o nome da propriedade
        category: cliente.clienteNome.split(" ")[0] // Extrai o primeiro nome // Altere conforme o nome da propriedade
    }));

    var series = chart.series.push(am5percent.PieSeries.new(root, {
        valueField: "value",
        categoryField: "category"
    }));
    /*var series = chart.series.push(
        am5percent.PieSeries.new(root, {
            valueField: "value",
            categoryField: "category",
            endAngle: 270,
            alignLabels: false
        })*/
    //);

    series.children.push(am5.Label.new(root, {
        centerX: am5.percent(50),
        centerY: am5.percent(50),
        text: "Valores",
        populateText: true,
        fontSize: "1.5em"
    }));

    series.slices.template.setAll({
        cornerRadius: 8
    })

    series.states.create("hidden", {
        endAngle: -90
    });

    series.labels.template.setAll({
        textType: "circular"
    });


    // Create the 2nd chart
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/
    var chart2 = chartContainer.children.push(
        am5percent.PieChart.new(root, {
            endAngle: 270,
            innerRadius: am5.percent(60)
        })
    );

    // Set data
    var data2 = GetTopClientesMaisCompras.map(cliente => ({
        value: cliente.totalComprado, // Altere conforme o nome da propriedade
        category: cliente.clienteNome.split(" ")[0] // Extrai o primeiro nome // Altere conforme o nome da propriedade
    }));

    var series2 = chart2.series.push(am5percent.PieSeries.new(root, {
        valueField: "value",
        categoryField: "category"
    }));
    /*var series2 = chart2.series.push(
        am5percent.PieSeries.new(root, {
            valueField: "value",
            categoryField: "category",
            endAngle: 270,
            alignLabels: false,
            tooltip: am5.Tooltip.new(root, {}) // a separate tooltip needed for this series
        })
    );*/

    series2.children.push(am5.Label.new(root, {
        centerX: am5.percent(50),
        centerY: am5.percent(50),
        text: "Quantidade",
        populateText: true,
        fontSize: "1.5em"
    }));

    series2.slices.template.setAll({
        cornerRadius: 8
    })

    series2.states.create("hidden", {
        endAngle: -90
    });

    series2.labels.template.setAll({
        textType: "circular"
    });


    // Duplicate interaction
    // Must be added before setting data
    series.slices.template.events.on("pointerover", function (ev) {
        var slice = ev.target;
        var dataItem = slice.dataItem;
        var otherSlice = getSlice(dataItem, series2);

        if (otherSlice) {
            otherSlice.hover();
        }
    });

    series.slices.template.events.on("pointerout", function (ev) {
        var slice = ev.target;
        var dataItem = slice.dataItem;
        var otherSlice = getSlice(dataItem, series2);

        if (otherSlice) {
            otherSlice.unhover();
        }
    });

    series.slices.template.on("active", function (active, target) {
        var slice = target;
        var dataItem = slice.dataItem;
        var otherSlice = getSlice(dataItem, series2);

        if (otherSlice) {
            otherSlice.set("active", active);
        }
    });

    // Same for the 2nd series
    series2.slices.template.events.on("pointerover", function (ev) {
        var slice = ev.target;
        var dataItem = slice.dataItem;
        var otherSlice = getSlice(dataItem, series);

        if (otherSlice) {
            otherSlice.hover();
        }
    });

    series2.slices.template.events.on("pointerout", function (ev) {
        var slice = ev.target;
        var dataItem = slice.dataItem;
        var otherSlice = getSlice(dataItem, series);

        if (otherSlice) {
            otherSlice.unhover();
        }
    });

    series2.slices.template.on("active", function (active, target) {
        var slice = target;
        var dataItem = slice.dataItem;
        var otherSlice = getSlice(dataItem, series);

        if (otherSlice) {
            otherSlice.set("active", active);
        }
    });


    // Set data
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
    series.data.setAll(data);

    // Set data
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
    series2.data.setAll(data2);


    function getSlice(dataItem, series) {
        var otherSlice;
        am5.array.each(series.dataItems, function (di) {
            if (di.get("category") === dataItem.get("category")) {
                otherSlice = di.get("slice");
            }
        });

        return otherSlice;
    }

    // Create legend
    var legend = root.container.children.push(am5.Legend.new(root, {
        x: am5.percent(50),
        centerX: am5.percent(50)
    }));


    // Trigger all the same for the 2nd series
    legend.itemContainers.template.events.on("pointerover", function (ev) {
        var dataItem = ev.target.dataItem.dataContext;
        var slice = getSlice(dataItem, series2);
        slice.hover();
    });

    legend.itemContainers.template.events.on("pointerout", function (ev) {
        var dataItem = ev.target.dataItem.dataContext;
        var slice = getSlice(dataItem, series2);
        slice.unhover();
    });

    legend.itemContainers.template.on("disabled", function (disabled, target) {
        var dataItem = target.dataItem.dataContext;
        var slice = getSlice(dataItem, series2);
        if (disabled) {
            series2.hideDataItem(slice.dataItem);
        }
        else {
            series2.showDataItem(slice.dataItem);
        }
    });

    legend.data.setAll(series.dataItems);

    series.appear(1000, 100);

    function ajustarRotulos() {
        const larguraTela = window.innerWidth;

        if (larguraTela < 768) { // Ajuste o valor conforme necessário
            series.labels.template.set("forceHidden", true);  // Oculta os rótulos
            series2.labels.template.set("forceHidden", true);  // Oculta os rótulos
        } else {
            series.labels.template.set("forceHidden", false); // Exibe os rótulos
            series2.labels.template.set("forceHidden", false); // Exibe os rótulos
        }
    }

    // Chama a função para definir o estado inicial
    ajustarRotulos();

    // Adiciona o listener para redimensionamento da tela
    window.addEventListener("resize", ajustarRotulos);

    // Remove o evento ao destruir o gráfico (evita vazamento de memória)
    root.events.on("disposed", function () {
        window.removeEventListener("resize", ajustarRotulos);
    });
}