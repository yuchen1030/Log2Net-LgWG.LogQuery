$(function () {
    //Widgets countabp.toast
    // $('.count-to').countTo();

    //Sales count to
    //$('.sales-count-to').countTo({
    //    formatter: function (value, options) {
    //        return '$' + value.toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, ' ').replace('.', ',');
    //    }
    //});


    $("#from-date").val(getDateStr(-7));
    $("#to-date").val(getDateStr(0));

    $("#rangeStartT").val(getDateStr(-1) + "T00:00:00");
    $("#rangeEndT").val(getDateStr(0) + "T23:59:59");


    $('#rtModalSpeed').val(abp.setting.get("Abp.Def.ChartRefreshSpeed"));
    $('#monitorPonitShow').val(abp.setting.get("Abp.Def.ChartWinSize"));
    $('#xAxisFormateCmb').selectpicker('val', abp.setting.get("Abp.Def.ChartXAxisFormate"));
    setCheckbox('chartWindowMode', isChartScrollMode(true));

    var monitorRandgeStr = abp.setting.get("Abp.Def.ChartMonitorRange").split('_');
    $("input:radio[name='monitorRange']").eq(monitorRandgeStr[0]).attr("checked", 'checked');

    setColorSelectMonitrRangeRadio().siblings('input.rangeNum').val(monitorRandgeStr[1]);

    var realTimeMode = abp.setting.get("Abp.Def.ChartRealTimeMode");
    setCheckbox('chartRealTimeMode', realTimeMode == "1");
    setRealTimeModalForm(realTimeMode == "1");

    getRealTimeModeText();

    logHub = $.connection.logMonitorHubMini; //[HubName("logMonitorHubMini")] 属性值
    abp.event.on('abp.signalr.connected', function () { //register for connect event
        transClientParaToServerHub();
    });

    //SignalR客户端方法，定时刷新曲线显示等
    logHub.client.updateLogMonitorDatas = function (logVMData) {
        var checkOrNot = isChartScrollMode(false);
        logVMData = $.parseJSON(logVMData);

        var bOK = checkWebServerNum(logVMData);
        refreshNumStatistics(logVMData);
        if (!bOK) {
            abp.notify.warn("当前监控中出现了可疑信息，请参考右上方的警告信息收件箱中的提示");
            if (parseInt($('#mailNotifyNumTxt').text()) > 20) {
                $('#rtModalSpeed').val(1200);
                getRealTimeModeText();
                transClientParaToServerHub();
                abp.notify.warn("当前监控中出现的告警提示超过20条，系统将自动降低刷新速度为20分钟");
            } else {
                getCmbOptions_Glob('/LogMonitor/GetAllApplicationList', 'appNameCmb', 'webSiteDiv', 'webAllSpan');
                getCmbOptions_Glob('/LogMonitor/GetAllServerNameList', 'serverNameCmb', 'serverDiv', 'serverAllSpan');
                getMonitorDataWhenInit();
            }
            return;
        }

        var serversData = logVMData.ServerData;
        for (var i = 0; i < serversData.length; i++) {
            var oneConfig = chartsConfig[i];
            var oneChart = chartPics[i];
            var serName = serversData[i].ServerName;
            var appName = serversData[i].AppName;
            var serData = serversData[i].Monitors;
            addOnePoint(oneChart, oneConfig, serData, checkOrNot);
        }

    }



});



function startTimerToRefreshChart() {

    if ($('#chartRealTimeMode').is(':checked')) {
        var sec = parseFloat($('#rtModalSpeed').val());
        if (sec > 0) {
            sec = sec * 1000;
            if (chartInterval != null) {
                clearInterval(chartInterval);
            }
            chartInterval = setInterval("showChartData()", sec);
        }
    }
}


var timerErrCnt = 0;
function showChartData() {
    var tttt = abp.signalr;
    var bRTMode = $('#chartRealTimeMode').is(':checked');
    if (!bRTMode || timerErrCnt > 3) {
        if (chartInterval != null) {
            clearInterval(chartInterval);
        }
        return;
    }

    try {
        logHub.server.getAllLogMonitorDatas($('#xAxisFormateCmb').val(), $('#rtModalSpeed').val(), $('#chartRealTimeMode').is(':checked'), abp.session.userId);
        timerErrCnt = 0;
    }
    catch (ex) {
        timerErrCnt++;
        console.log("start getAllLogMonitorDatas from server error: " + ex.message);
    }




}



var chartInterval = null;
var logHub = null;


function transClientParaToServerHub() {
    logHub.server.initParasTrans($('#xAxisFormateCmb').val(), $('#rtModalSpeed').val(), $('#chartRealTimeMode').is(':checked'), abp.session.userId);
}

//在定时刷新时，检查网站和服务器数量，在变化时发送通知，并停止定时任务
function checkWebServerNum(logVMData) {
    var dbWebs = logVMData.WebSites;
    var dbServers = logVMData.Servers;
    var bok1 = checkNumAndDetail(dbWebs, 'webAllSpan', 'webSiteDiv', '网站');
    var bok2 = checkNumAndDetail(dbServers, 'serverAllSpan', 'serverDiv', '服务器');
    var bOK = bok1 & bok2;
    if (!bOK) {
        playStopWarningMusic(true);
    }
    return bOK;
}


function checkNumAndDetail(dbWebs, numID, divID, item) {
    var uiWebNum = $('#' + numID).html();
    var uiWebs = $('#' + divID).attr("title");
    var arrTemp = uiWebs.split(/[\r\n、]/);
    var arr2 = new Array();
    for (var i = 0; i < arrTemp.length; i++) {
        if (arrTemp[i].length >= 3) {
            arr2.push(arrTemp[i].split('[')[0]);
        }
    }
    var bOK = true;
    var arr3 = differSet(dbWebs, arr2);
    if (arr3.length > 0) { //arr3非空，说明db中有客户端无，是新加入的。
        var subText = "新加入监控";
        for (var i = 0; i < arr3.length; i++) {
            var mainText = item + "【" + arr3[i] + "】";
            createWarnningMsg(mainText, subText);
        }
        bOK = false;
    }

    var arr3 = differSet(arr2, dbWebs);
    if (arr3.length > 0) { //arr3非空，说明db中无客户端有，是新移除的。
        var subText = "失去监控";
        for (var i = 0; i < arr3.length; i++) {
            var mainText = item + "【" + arr3[i] + "】";
            createWarnningMsg(mainText, subText);
        }
        bOK = false;
    }
    return bOK;

}

//求差集
function differSet(arr1, arr2, texts) {
    var arr3 = new Array();
    if (arr2 == null) {
        arr2 = texts.split('');
    }
    for (var i = 0; i < arr1.length; i++) {
        var flag = true;
        for (var j = 0; j < arr2.length; j++) {
            if (arr1[i] == arr2[j])
                flag = false;
        }
        if (flag) {
            arr3.push(arr1[i]);
        }
    }
    return arr3;


}



//动态创建告警信息
function createWarnningMsg(mainText, subText) {
    var motherDiv = document.getElementById('mailNotifyDetail');
    var time = (new Date()).Format("d H:m:s");
    var div1 = document.createElement("li");
    div1.className = 'mailListCss';
    div1.innerHTML = '<a href="#" class="mailListCss"><h4><small class="mailPicCss"><i class="fa fa-envelope-o"></i></small><span style = "padding:0 5px" > ' + mainText
        + '</span > <small><i class="fa fa-clock-o"></i> ' + time + ' </small></h4><p>' + subText + '</p></a>';
    motherDiv.appendChild(div1);
    $('#mailNotifyNumTxt').text(parseInt($('#mailNotifyNumTxt').text()) + 1);
}


var timeFormat = 'MM/DD/YYYY HH:mm:ss';
timeFormat = 'YYYY-MM-DD HH:mm:ss';
function newDate(days, bDate) {
    if (bDate) {//当设置x轴格式为时间时使用
        var resultDate = moment().add(days, 'd').toDate();
        return resultDate;
    }
    return moment().add(days, 'd').format("M-D H:m:s");

}

function newDateString(days) {
    var resultDate = moment().add(days, 'd').format(timeFormat);
    return resultDate;
}

var color = Chart.helpers.color;
var chartsConfig = [];
var chartPics = [];

//$(function () {

window.onload = function () {

    getMonitorDataWhenInit();

    var oTable = new TableInit();
    oTable.Init();

    var tttt = abp.auth.grantedPermissions;

    getCmbOptions_Glob('/LogMonitor/GetAllApplicationList', 'appNameCmb', 'webSiteDiv', 'webAllSpan');


    getCmbOptions_Glob('/LogMonitor/GetAllServerNameList', 'serverNameCmb', 'serverDiv', 'serverAllSpan');



    $(".date").datepicker({
        language: "zh-CN",
        autoclose: true,//选中之后自动隐藏日期选择框
        //clearBtn: true,//清除按钮
        //todayBtn: true,//今日按钮
        format: "yyyy-mm-dd"//日期格式，
    });

    $('input[type=radio][name=monitorRange]').change(function () {
        setColorSelectMonitrRangeRadio();
        //var curRadioDiv = $(this).parent();
        //$(this).parent().siblings().css("background", "");
        //$(this).parent().css("background", "#D8F2F4");
    });

    $("#chartRefreshBtn").click(function () {
        getMonitorDataWhenInit();
    });

    $('#appNameCmb').bind('change', function () {
        Search();
    });

    $('#serverNameCmb').bind('change', function () {
        Search();
    });


    $("#btn-search").click(function () {
        Search();
    });

    $(".ckRealTime").click(function () {
        var bCK = $(this).is(':checked');
        setRealTimeModalForm(bCK);
        setCheckbox('chartRealTimeMode', bCK);
        $('#realTimeModal').modal('show');
    });

    $("#rtModalBtnCancel").click(function () {
        var bCK = isContains($('#realTimeModeShowBtn').text(), "实时");
        setCheckbox('chartRealTimeMode', bCK);
        setRealTimeModalForm(bCK);
    });




    var _userService = abp.services.app.user;
    var _$modalRT = $('#realTimeModal');
    var _$formRT = _$modalRT.find('form');

    _$formRT.find('button[type="submit"]').click(function (e) {
        e.preventDefault();
        if (!getRealTimeModeText()) {
            return;
        }
        _$modalRT.modal('hide');
        var result = getMonitorRangeAndPoint(true);
        if (result != "") {
            getMonitorDataWhenInit();
        }

        return;
        var user = _$formRT.serializeFormToObject(); //serializeFormToObject is defined in main.js

        abp.ui.setBusy(_$modalRT);
        _userService.create(user).done(function () {
            _$modalRT.modal('hide');
            location.reload(true); //reload page to see new user!
        }).always(function () {
            abp.ui.clearBusy(_$modalRT);
        });
    });



    var _userService = abp.services.app.user;
    var _$modalRange = $('#monitorRangeModal'); 
    var _$formRange = _$modalRange.find('form');

    _$formRange.find('button[type="submit"]').click(function (e) {
        e.preventDefault();

        var result = getMonitorRangeAndPoint(true);
        if (result != "") {
            _$modalRange.modal('hide');
            getMonitorDataWhenInit();
        }

        return;
        var user = _$formRange.serializeFormToObject(); //serializeFormToObject is defined in main.js

        abp.ui.setBusy(_$modalRange);
        _userService.create(user).done(function () {
            _$modalRange.modal('hide');
            location.reload(true); //reload page to see new user!
        }).always(function () {
            abp.ui.clearBusy(_$modalRange);
        });
    });


};



function getRealTimeModeText() {
    var bCK = $('#modalRealTimeMode').is(':checked');
    var modeStr = bCK ? '实时模式' : '查询模式';
    if (bCK) {
        var speedShow = parseInt($('#rtModalSpeed').val());
        if (isNaN(speedShow)) {
            abp.message.warn('请在刷新速度框中填入合法的数字');
            return false;
        }
        var bScroll = $('#chartWindowMode').is(':checked');
        modeStr += "，每 " + speedShow + " 秒刷新，窗口" + (bScroll ? "滑动" : "扩张");
    }
    $('#realTimeModeShowBtn').text(modeStr);
    return true;
}

function formatLineProduct(lineProduct) {
    return lineProduct;
}


function isContains(text, char) {
    return text.indexOf(char) >= 0;
}

function setCheckbox(ckID, value) {
    // $('#chartWindowMode').prop('checked', isChartScrollMode(true));
    $('#' + ckID).prop('checked', value);
}


function setRealTimeModalForm(value) {
    setCheckbox('modalRealTimeMode', value);
    if (value) {
        $('#realTimeModalDiv').show();
    } else {
        $('#realTimeModalDiv').hide();
    }
}


function isChartScrollMode(fromServer) {
    if (!fromServer) {
        return $('#chartWindowMode').is(':checked');
    } else {
        return abp.setting.get("Abp.Def.ChartScrollMode").split('_') == "1";
    }
}



//创建一个chart.js型chart并初始化之
function createAndInitOneChart(motherDiv, chartIDPre, index) {
    var sonDiv = document.createElement("div");
    sonDiv.className = 'col-xs-12 col-sm-12 col-md-6 col-lg-6 chartDivCss';
    var chartID = chartIDPre + index;
    sonDiv.innerHTML = '<canvas id="' + chartID + '"></canvas>';
    motherDiv.appendChild(sonDiv);
    var oneConfig = {
        type: 'line',
        pointDot: true,
        options: {
            responsive: true,  //add
            hoverMode: 'index',//add
            stacked: false,//add

            scales: {
                yAxes: [{
                    type: "linear", // only linear but allow scale type registration. This allows extensions to exist solely for log scale for instance
                    display: true,
                    position: "left",
                    id: "y-axis-1",
                    scaleLabel: {
                        display: true,
                        labelString: 'CPU / 内存使用率(%)'
                    }
                }, {
                    type: "linear", // only linear but allow scale type registration. This allows extensions to exist solely for log scale for instance
                    display: true,
                    position: "right",
                    id: "y-axis-2",
                    scaleLabel: {
                        display: true,
                        labelString: '线程数 / 在线人数'
                    },
                    // grid line settings
                    gridLines: {
                        drawOnChartArea: false, // only want the grid lines for one axis to show up
                    },
                }],
            },
        }
    };
    chartsConfig.push(oneConfig);


}

//动态创建div和canvas，用于显示chart.js型的曲线图
function createChartDiv(num) {
    var motherDiv = document.getElementById('chartsDiv');
    $(motherDiv).empty();
    chartsConfig = [];
    chartPics = [];

    var chartIDPre = 'canvasServer_';
    for (var i = 0; i < (num + 1) / 2; i++) {
        if (2 * i >= num) {
            break;
        }
        var div1 = document.createElement("div");
        div1.className = 'row clearfix';
        createAndInitOneChart(div1, chartIDPre, 2 * i);
        if (2 * i + 1 < num) {
            createAndInitOneChart(div1, chartIDPre, 2 * i + 1);
        }
        motherDiv.appendChild(div1);
    }

    for (var i = 0; i < num; i++) {
        chartPics.push(new Chart($('#' + chartIDPre + i)[0].getContext('2d'), chartsConfig[i]));
    }

}

function refreshNumStatistics(data) {
    $('#webAllSpan').html(data.WebSites.length);
    $('#serverAllSpan').text(data.Servers.length);
    $('#dayLogAllSpan').text(data.LogNum);
    $('#refreshT').html(data.Time);
}

//进入页面时，获取监控数据初始化chart
function getMonitorDataWhenInit() {
    var result = getMonitorRangeAndPoint();
    if (result == "") {
        return;
    }
    try {
        transClientParaToServerHub();
    }
    catch (e) {

    }

    $.get('/LogMonitor/GetMonitorChartData', { "range": result, 'userId': abp.session.userId }, function (dbData) { //根据情况修改
        var serversData = dbData.Result.ServerData;

        if (serversData.length == 0) {
            playStopWarningMusic(true);
            var subText = "当前条件下，无监控数据~";
            createWarnningMsg('页面初始化查询', subText);
            abp.notify.warn(subText);
        }

        refreshNumStatistics(dbData.Result);

        createChartDiv(serversData.length);

        for (var i = 0; i < serversData.length; i++) {
            var oneConfig = chartsConfig[i];
            var oneChart = chartPics[i];
            var serName = serversData[i].ServerName;
            var appName = serversData[i].AppName;
            var serData = serversData[i].Monitors;

            var newColor1 = GetOneColor(0);
            var newColor2 = GetOneColor(1);
            var newColor3 = GetOneColor(2);
            var newColor4 = GetOneColor(3);

            var oneDataset_cpu = { label: "cpu使用率", backgroundColor: newColor1, borderColor: newColor1, data: [], fill: false, yAxisID: "y-axis-1" };
            var oneDataset_mem = { label: "内存使用率", backgroundColor: newColor2, borderColor: newColor2, data: [], fill: false, yAxisID: "y-axis-1" };
            var oneDataset_thr = { label: "线程数", backgroundColor: newColor3, borderColor: newColor3, data: [], fill: false, yAxisID: "y-axis-2" };
            var oneDataset_onLine = { label: "在线人数", backgroundColor: newColor4, borderColor: newColor4, data: [], fill: false, yAxisID: "y-axis-2" };

            oneConfig.options.title = { display: true, text: "【" + serName + "】服务器监控" };
            for (var index = 0; index < serData.length; ++index) {
                var xData = serData[index].LogTime;
                oneConfig.data.labels.push(xData);
                initOneChartDataFunc(oneDataset_cpu, xData, serData[index].CpuUsage);
                initOneChartDataFunc(oneDataset_mem, xData, serData[index].MemoryUsage);
                initOneChartDataFunc(oneDataset_thr, xData, serData[index].CurProcThreadNum);
                initOneChartDataFunc(oneDataset_onLine, xData, serData[index].OnlineCnt);
            }

            oneConfig.data.datasets.push(oneDataset_cpu);
            oneConfig.data.datasets.push(oneDataset_mem);
            oneConfig.data.datasets.push(oneDataset_thr);
            oneConfig.data.datasets.push(oneDataset_onLine);
            oneChart.update();
        }

        startTimerToRefreshChart();
    });
}


//chart.js中，在最后的位置添加一个点
function addOnePoint(curChart, config, serversDatas, bRemoveFirst) {
    if (curChart == undefined || config == undefined) {
        return;
    }
    var winSize = parseInt($('#monitorPonitShow').val());
    for (var i = 0; i < serversDatas.length; i++) {
        if (bRemoveFirst && config.data.labels.length >= winSize) {
            removeFirstPoint(curChart, config);
        }
        var serversData = serversDatas[i];
        config.data.labels.push(serversData.LogTime);

        var xData = serversData.LogTime;
        addOneChartDataFunc(config, 0, xData, serversData.CpuUsage);
        addOneChartDataFunc(config, 1, xData, serversData.MemoryUsage);
        addOneChartDataFunc(config, 2, xData, serversData.CurProcThreadNum);
        addOneChartDataFunc(config, 3, xData, serversData.OnlineCnt);
    }
    curChart.update();

    if (!bRemoveFirst) {
        $('#monitorPonitShow').val(Math.max(config.data.labels.length, parseInt($('#monitorPonitShow').val())));
    }


}

//初始化chart Data的两种方式
function initOneChartDataFunc(dataset, xData, yData, objMode) {
    if (!objMode) {
        dataset.data.push(yData);
    } else {
        dataset.data.push({ x: xData, y: yData });
    }
}

//添加chart Data的两种方式
function addOneChartDataFunc(config, index, xData, yData) {
    if (typeof config.data.datasets[index].data[0] !== 'object') {// number或object
        config.data.datasets[index].data.push(yData);
    } else {
        config.data.datasets[index].data.push({ x: xData, y: yData });
    }
}

//chart.js中，移除中第一个点
function removeFirstPoint(curChart, config) {
    config.data.labels.splice(0, 1); // remove the label first
    config.data.datasets.forEach(function (dataset) {
        dataset.data.shift();//移除第一个点
    });
    //以下为移除最后一个点的方法
    //var tt = config.data.labels.splice(-1, 1); // remove the label last
    //config.data.datasets.forEach(function (dataset) {
    //    dataset.data.pop();//移除最后一个点
    //}); 
}

//设置monitorRange选中项的背景色
function setColorSelectMonitrRangeRadio() {
    var radioControl = $("input[name='monitorRange']:checked");
    var curRadioDiv = radioControl.parent();
    radioControl.parent().siblings().css("background", "");
    radioControl.parent().css("background", "#D8F2F4");
    return radioControl;
}

//获取监控范围和点数
function getMonitorRangeAndPoint(modal) {
    var radioControl = $("input[name='monitorRange']:checked");
    var result = radioControl.val();
    var inputControls = radioControl.siblings('input');
    for (var i = 0; i < inputControls.length; i++) {
        var curText = inputControls[i].value;
        if (curText == '') {
            abp.message.warn('请在你的选项上填入合法的值。');
            return "";
        }
        if (inputControls.length == 1) {
            curText = parseFloat(curText);
            if (isNaN(curText)) {
                abp.message.warn('请在你的选项上填入合法的值。');
                return "";
            }
        }
        result += "_" + curText;
    }

    var labelControls = radioControl.siblings('label');
    var showText = '';
    if (labelControls.length == 1) {//是当天
        showText = labelControls[0].innerText;
    } else if (inputControls.length == 2) { //是自定义时间
        showText = labelControls[0].innerText + inputControls[0].value.replace('T', ' ') + labelControls[1].innerText + inputControls[1].value.replace('T', ' ');
    }
    else {  //最近xx小时/最近xx点
        showText = labelControls[0].innerText + inputControls[0].value + labelControls[1].innerText;
    }
    $('#monitorRangeShow').text(showText);
    if (!modal) {
        var pointShow = parseInt($('#monitorPonitShow').val());
        if (isNaN(pointShow)) {
            abp.message.warn('请在显示点数框中填入合法的数字');
            return "";
        }
        result += "_" + pointShow + "_" + $('#xAxisFormateCmb').val();
    }
    return result;

}

//以下为bootstrapTable搜索相关
var selectAppID = -1;
var selectHost = "";
var TableInit = function () {
    var oTableInit = new Object();
    //初始化Table
    oTableInit.Init = function () {
        $('#logMonitorTable').bootstrapTable({
            url: '/LogMonitor/GetLogMonitorData',         //请求后台的URL（*）
            method: 'get',                      //请求方式（*）
            toolbar: '#toolbar',                //工具按钮用哪个容器
            striped: true,                      //是否显示行间隔色
            cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
            pagination: true,                   //是否显示分页（*）
            sortable: true,                     //是否启用排序
            sortOrder: "desc",                   //排序方式
            queryParams: oTableInit.queryParams,//传递参数（*）
            sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
            pageNumber: 1,                       //初始化加载第一页，默认第一页
            pageSize: 10,                       //每页的记录行数（*）
            pageList: [5, 10, 25, 50, 100],        //可供选择的每页的行数（*）
            search: false,                       //是否显示表格搜索，自定义搜索，不使用自带的
            contentType: "application/x-www-form-urlencoded",
            strictSearch: true,
            showColumns: true,                  //是否显示所有的列
            showRefresh: true,                  //是否显示刷新按钮
            minimumCountColumns: 2,             //最少允许的列数
            clickToSelect: true,                //是否启用点击选中行
            //  height: 700,                        //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
            uniqueId: "Id",                     //每一行的唯一标识，一般为主键列
            showToggle: true,                    //是否显示详细视图和列表视图的切换按钮
            cardView: false,                    //默认显示详细视图
            detailView: false,                   //是否显示父子表
            columns: [
                {
                    field: 'Id',
                    title: '编号',
                    sortable: true
                }, {
                    field: 'LogTime',
                    title: '时间',
                    sortable: true,
                    formatter: function (value, row, index) {
                        return changeDateFormat(value);
                    }
                }, {
                    field: 'SysName',
                    title: '系统名称',
                    sortable: true
                }, {
                    field: 'ServerHost',
                    title: '服务器名称',
                    sortable: true
                }, {
                    field: 'ServerIP',
                    title: '服务器IP',
                    sortable: true
                }, {
                    field: 'OnlineCnt',
                    title: '在线人数',
                    sortable: true
                }, {
                    field: 'AllVisitors',
                    title: '历史访客',
                    sortable: true,
                }, {
                    field: 'RunHours',
                    title: '运行时长',
                    sortable: true
                }, {
                    field: 'DiskSpace',
                    title: '剩余磁盘空间'

                }, {
                    field: 'CpuUsage',
                    title: 'CPU(%)',
                    sortable: true
                }, {
                    field: 'MemoryUsage',
                    title: '内存(%)',
                    sortable: true
                }, {
                    field: 'ProcessNum',
                    title: '进程数',
                    sortable: true
                }, {
                    field: 'ThreadNum',
                    title: '线程数',
                    sortable: true
                }, {
                    field: 'CurProcThreadNum',
                    title: 'IIS线程数',
                    sortable: true
                }, {
                    field: 'CurProcMem',
                    title: 'IIS内存(M)',
                    sortable: true
                }, {
                    field: 'CurSubProcMem',
                    title: '当前程序内存(M)',
                    sortable: true
                }

            ],
            //rowStyle: function (row, index) {
            //    var classesArr = ['success', 'info'];
            //    var strclass = "";
            //    if (index % 2 === 0) {//偶数行
            //        strclass = classesArr[0];
            //    } else {//奇数行
            //        strclass = classesArr[1];
            //    }
            //    return { classes: strclass };
            //},//隔行变色

            onLoadSuccess: function (data) {
                $("#logMonitorTable").bootstrapTable("load", data.Result);
                // return false;
            },

            onLoadError: function (data) {
                // return false;
            },


        });

    };


    //得到查询的参数
    oTableInit.queryParams = function (params) {
        var temp = {   //这里的键的名字和控制器的变量名必须一直，这边改动，控制器也需要改成一样的
            limit: params.limit,   //页面大小
            offset: params.offset,
            sortby: params.sort, //排序字段
            sortway: params.order, //升序降序
            host: $('#serverNameCmb').val(),
            app: $('#appNameCmb').val() == null ? -1 : $('#appNameCmb').val(),
            from: $('#from-date').val(),
            to: $('#to-date').val()

        };
        return temp;
    };
    return oTableInit;
};


function Search() {
    refreshBootstrapTable('logMonitorTable');
}



