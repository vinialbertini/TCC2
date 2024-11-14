function GeraGraficoTopProdutos(GetTopProdutos) {

    // Create root element
    // https://www.amcharts.com/docs/v5/getting-started/#Root_element
    var root = am5.Root.new("chartdiv2");


    // Set themes
    // https://www.amcharts.com/docs/v5/concepts/themes/
    root.setThemes([
        am5themes_Animated.new(root)
    ]);


    // Create chart
    // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/
    var chart = root.container.children.push(am5percent.PieChart.new(root, {}));

    // Set data
    var data = GetTopProdutos.map(produto => ({
        value: produto.quantidadeVendida, // Altere conforme o nome da propriedade
        category: produto.produtoNome // Altere conforme o nome da propriedade
    }));

    // Create series
    var series = chart.series.push(am5percent.PieSeries.new(root, {
        valueField: "value",
        categoryField: "category"
    }));

    // Set data for the series
    series.data.setAll(data);

    function ajustarRotulos() {
        const larguraTela = window.innerWidth;

        if (larguraTela < 768) { // Ajuste o valor conforme necessário
            series.labels.template.set("forceHidden", true);  // Oculta os rótulos
        } else {
            series.labels.template.set("forceHidden", false); // Exibe os rótulos
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
    // Play initial series animation
    // https://www.amcharts.com/docs/v5/concepts/animations/#Animation_of_series
    series.appear(1000, 100);
}