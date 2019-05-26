
document.write("<script src='/js/bootstrap-datepicker.js' type='text/javascript'></script>");
document.write("<script src='/js/bootstrap-dialog.min.js' type='text/javascript'></script>");
document.write("<script src='http://cdnjs.cloudflare.com/ajax/libs/bootstrap-table/1.11.0/bootstrap-table.min.js'></script>");
document.write("<script src='http://cdnjs.cloudflare.com/ajax/libs/bootstrap-table/1.11.0/locale/bootstrap-table-zh-CN.min.js'></script>");





function refreshBootstrapTable(dtID) {
    $('#' + dtID).bootstrapTable('refresh');
}


//下拉列表的赋值
function getCmbOptions_Glob(url, cmbID, numDivID, numSpanID) {
    $.post(url, {}, function (serverData) { //根据情况修改
        //if (!checkLoginOK(serverData)) {
        //    return;
        //}
        var teamData = serverData.Result;
        var options = '';
        var texts = '';
        var nums = 1;
        for (var I in teamData) {
            if (teamData[I].Value != undefined){
                options += " <option value = '" + teamData[I].Value + "'>" + teamData[I].Text + " </option>";
                if (teamData[I].Value != '-1' ) {
                    texts += nums + "、"+  teamData[I].Text + "\r\n";
                    nums++;
                }
            }
        }
        $('#' + cmbID).html($('#' + cmbID).html() + options);  //根据情况修改
        $('#' + cmbID).selectpicker('refresh');
        if (numDivID != undefined) {
            $('#' + numDivID).attr("title", texts);   
        }
        if (numSpanID != undefined) {
            $('#' + numSpanID).html(nums-1);
        }
    });
}


function changeDateFormat(cellval) {
    if (cellval == null) {
        return "";
    }
    cellval = cellval.replace(/T/g, " ");
    return cellval;
    cellval = cellval.replace(new RegExp(/-/gm), "/");
    var offlineTimeStr = new Date(cellval);
    return offlineTimeStr;
}


//获取yyyy-MM-dd格式的日期
function getDateStr(AddDayCount) {
    if (AddDayCount == undefined) {
        AddDayCount = 0;
    } else {
        AddDayCount = parseInt(AddDayCount, 10);
    }
    var dd = new Date();
    dd.setDate(dd.getDate() + AddDayCount);
    var y = dd.getFullYear();
    var m = dd.getMonth() + 1;
    var d = dd.getDate();
    var curyyyyMMdd = y + "-" + (m < 10 ? "0" : "") + m + "-" + (d < 10 ? "0" : "") + d
    return curyyyyMMdd;
}


// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "H+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "f": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}


