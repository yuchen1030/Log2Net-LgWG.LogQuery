// 简单替换方法
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}
$(function () {

    $('#btnControl').click(function () {

    });

    var ticker = $.connection.lineProductTickerMini, //[HubName("lineProductTickerMini")] 属性值
        up = '▲',
        down = '▼',
        $lineProductTable = $('#lineProductTable'),
        $lineProductTableBody = $lineProductTable.find('tbody'),
        rowTemplate = '<tr data-symbol="{Symbol}"><td>{Symbol}</td><td>{Num1}</td><td>{Num2}</td><td>{Num3}</td><td>{Num4}</td></tr>';

    function formatLineProduct(lineProduct) {
        return lineProduct;
    }

    function init() {
        ticker.server.getAllLineProducts().done(function (lineProducts) {
            $lineProductTableBody.empty();
            $.each(lineProducts, function () {
                var lineProduct = formatLineProduct(this);
                $lineProductTableBody.append(rowTemplate.supplant(lineProduct));
            });
            option_GL.series[0].data = new Array(lineProducts[0].Num1, lineProducts[0].Num2, lineProducts[0].Num3, lineProducts[0].Num4);
            option_GL.series[1].data = new Array(lineProducts[1].Num1, lineProducts[1].Num2, lineProducts[1].Num3, lineProducts[1].Num4);
            setSeriesData(option_GL, myChart_GL);
        });
    }

    ticker.client.updateProductNumMul = function (lineProducts) {   //每次更新多条记录
        $.each(lineProducts, function () {
            var lineProduct = formatLineProduct(this);
            $row = $(rowTemplate.supplant(lineProduct));
            $lineProductTableBody.find('tr[data-symbol=' + lineProduct.Symbol + ']').replaceWith($row);
        });
        option_GL.series[0].data = new Array(lineProducts[0].Num1, lineProducts[0].Num2, lineProducts[0].Num3, lineProducts[0].Num4);
        option_GL.series[1].data = new Array(lineProducts[1].Num1, lineProducts[1].Num2, lineProducts[1].Num3, lineProducts[1].Num4);
        setSeriesData(option_GL, myChart_GL);  //useEChart();

    }

    // 开始client和server连接
    $.connection.hub.start().done(init);


});