// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(H)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符
// 毫秒(f)可以用1-3个占位符，几位占位符就代表几位精度
// 上/下午(t/T)、星期(w/W)只需要一个占位符
// 例子： 
// (new Date()).format("yyyy-MM-dd HH:mm:ss.fff") ==> 2017-02-08 15:09:04.423 
// (new Date()).format("yyyy-M-d h:m:s.ff")      ==> 20017-2-8 3:9:4.42
Date.prototype.format = function (fmt) {
    // 默认时间格式如：2017/01/01 23:23:23.435
    if (typeof (fmt) == 'undefined') fmt = 'yyyy/MM/dd HH:mm:ss.fff';

    var zeroize = function (value, length) {
        value = String(value);
        while (value.length < length) {
            value = '0' + value;
        }
        return value;
    };

    var o = {
        "M+": this.getMonth() + 1, // 月份 
        "d+": this.getDate(), // 日 
        "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, // 12小时制小时
        "H+": this.getHours(), // 24小时制小时 
        "m+": this.getMinutes(), // 分 
        "s+": this.getSeconds(), // 秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), // 季度 
        "t": this.getHours() < 12 ? 'am' : 'pm',
        "T": this.getHours() < 12 ? 'AM' : 'PM',
        "w": ['Sun', 'Mon', 'Tue', 'Wed', 'Thr', 'Fri', 'Sat'][this.getDay()],
        "W": ['星期日', '星期一', '星期二', '星期三', '星期四', '星期五', '星期六'][this.getDay()]
    };
    // 年
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, String(this.getFullYear()).substr(4 - RegExp.$1.length));
    }
    // 毫秒
    if (/(f+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, zeroize(this.getMilliseconds(), 3).substr(0, RegExp.$1.length));
    }
    // 其他
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) {
            fmt = fmt.replace(RegExp.$1, zeroize(o[k], Math.min(RegExp.$1.length, 2)));
        }
    return fmt;
};

// 是否手机号
String.prototype.isMobilePhone = function () {
    return /^1[345789][0-9]\d{8}$/.test(this);
};

// 是否邮箱
String.prototype.isEmail = function () {
    return /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/.test(this);
};

// 是否中文姓名
String.prototype.isChineseName = function () {
    return /^[\u4e00-\u9fa5]{2,10}$/.test(this);
};

// 是否密码
String.prototype.isPassword = function () {
    return /^[a-zA-Z][a-zA-Z0-9_]{7,15}$/.test(this);
};

// 是否身份证号
String.prototype.isIdentityCard = function () {
    return /^(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$)|(^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[Xx])$)$/.test(this);
};

// 正整数
String.prototype.isPositiveInt = function () {
    return /^[1-9]\d*$/.test(this);
};

// 正数
String.prototype.isPositiveNumber = function () {
    return /^[+]{0,1}(\d+)$|^[+]{0,1}(\d+\.\d+)$/.test(this);
};

// 字符串转Date，支持2017/01/01 12:00:00.001或/Date(1486525645937)/的时间字符串
String.prototype.convertToDate = function () {
    if (this.indexOf('Date(') >= 0) {
        return eval('new ' + this.replace(/\//g, ''));
    } else {
        // 毫秒截取后单独设置，避免ios无法支持毫秒直接转换（android无此问题）
        var index = this.lastIndexOf('.');
        if (index) {
            var milliSeconds = this.length == index + 1 ? 0 : parseInt(this.substr(index + 1));
            var date = new Date(Date.parse(this.substr(0, index)));
            date.setMilliseconds(milliSeconds);
            return date;
        }
        return new Date(Date.parse(this));
    }
};

var App = function () {
    this.imHost = "";
    this.socket = null;
    this.imCallbacks = [];
};

App.prototype.isBlank = function (text) {
    if (text == null || typeof (text) == 'undefined') return true;
    text = text.toString();
    var whitespace = "[\\x20\\t\\r\\n\\f]";
    var rtrim = new RegExp("^" + whitespace + "+|((?:^|[^\\\\])(?:\\\\.)*)" + whitespace + "+$", "g");
    text = text.replace(rtrim, "");
    return text === "";
};

App.prototype.startLoading = function (waitingOnTop) {
    if (waitingOnTop && top.layui) {
        top.layui.layer.load(2);
    } else {
        layui.layer.load(2);
    }
};

App.prototype.endLoading = function (waitingOnTop) {
    if (waitingOnTop && top.layui) {
        top.layui.layer.closeAll("loading");
    } else {
        layui.layer.closeAll("loading");
    }
};

App.prototype.showError = function (msg, showOnTop) {
    if (typeof (showOnTop) == "undefined") {
        showOnTop = true;
    }
    var obj = showOnTop && top.layui ? top.layui.layer : layui.layer;
    obj.alert(msg,
        {
            icon: 2,
            title: false,
            btn: false,
            shadeClose: true
        });
};

App.prototype.showWarning = function (msg, showOnTop) {
    if (typeof (showOnTop) == "undefined") {
        showOnTop = true;
    }
    var obj = showOnTop && top.layui ? top.layui.layer : layui.layer;
    obj.msg(msg, {
        icon: 0,
        time: 2500
    });
};

App.prototype.showSuccess = function (msg, showOnTop) {
    if (typeof (showOnTop) == "undefined") {
        showOnTop = true;
    }
    var obj = showOnTop && top.layui ? top.layui.layer : layui.layer;
    obj.msg(msg, {
        icon: 1,
        time: 1500
    });
};

App.prototype.showInfo = function (msg, showOnTop) {
    if (typeof (showOnTop) == "undefined") {
        showOnTop = true;
    }
    var obj = showOnTop && top.layui ? top.layui.layer : layui.layer;
    obj.msg(msg);
};

App.prototype.invokeService = function (option) {
    option = option || {};
    option.type = option.type || "POST";
    option.data = option.data || {};
    option.dataType = option.dataType || "json";
    option.contentType = option.contentType || "application/json; charset=utf-8";
    if (typeof (option.async) == "undefined") {
        option.async = true;
    }
    if (typeof (option.waiting) == "undefined") {
        option.waiting = true;
    }
    if (typeof (option.waitingOnTop) == "undefined") {
        option.waitingOnTop = true;
    }
    if (option.waiting) {
        this.startLoading(option.waitingOnTop);
    }
    var self = this;

    var ajaxOption = {
        type: option.type,
        data: JSON.stringify(option.data),
        dataType: option.dataType,
        url: option.url,
        contentType: option.contentType,
        async: option.async,
        success: function (data) {
            if (option.success) {
                option.success(data);
            }
            if (option.waiting) {
                self.endLoading(option.waitingOnTop);
            }
        },
        error: function (e) {
            if (option.waiting) {
                self.endLoading(option.waitingOnTop);
            }
            if (option.error) {
                option.error(e);
            }
            var error = JSON.parse(e.responseText);
            self.showError(error.Message);
        }
    };

    $.ajax(ajaxOption);
};

App.prototype.findArrayItem = function (arrayList, field, value, successCallback, failCallback) {
    var i = -1;
    for (var index in arrayList) {
        if (arrayList[index][field] == value) {
            i = index;
            break;
        }
    }
    if (i >= 0) {
        // 找到
        if (successCallback) {
            successCallback(arrayList[i], i);
        }
        return true;
    } else {
        // 未找到
        if (failCallback) {
            failCallback();
        }
        return false;
    }
};

App.prototype.getQueryStringByName = function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result == null || result.length < 1) {
        return "";
    }
    return result[1];
};

App.prototype.setStorage = function (id, obj) {
    if (obj) {
        localStorage.setItem(id, JSON.stringify(obj));
    } else {
        localStorage.removeItem(id);
    }
};

App.prototype.getStorage = function (id) {
    var s = localStorage.getItem(id);
    if (this.isBlank(s)) return null;
    return JSON.parse(s);
};

App.prototype.tapEnter = function (element, callback) {
    $(element).keydown(function (e) {
        if (e.keyCode === 13) {
            if (callback) {
                callback();
            }
        }
    });
};

App.prototype.disableBack = function () {
    //防止页面后退
    if (window.history && window.pushState) {
        $(window).on("popstate",
            function () {
                window.history.pushState("forward", null, null);
                window.history.forward(1);
            });
        // 这两行是为了兼容IE
        window.history.pushState("forward", null, null);
        window.history.forward(1);
    }
};

App.prototype.getFormJson = function (selector) {
    selector = selector || "form";
    var tempArray = $(selector).serializeArray();
    var obj = {};
    for (var i = 0; i <= tempArray.length - 1; i++) {
        obj[tempArray[i].name] = tempArray[i].value;
    }
    return obj;
};

App.prototype.setFormData = function (data) {
    for (var field in data) {
        var item = $("[name='" + field + "']");
        if (item) {
            if ($(item).is("input")) {
                switch ($(item).attr("type")) {
                case "radio":
                case "checkbox":
                    $(item)[0].checked = data[field];
                    break;
                default:
                    $(item).val(data[field]);
                    break;
                }
            } else if ($(item).is("textarea")) {
                $(item).text(data[field]);
            } else if ($(item).is("select")) {
                var option = $(item).find("option[value='" + data[field] + "']");
                if (option) {
                    option[0].selected = true;
                }
            } else {
                $(item).html(data[field]);
            }
        }
    }
};

App.prototype.verifyInput = function () {
    var result = true;
    var verify = layui.form.config.verify,
        danger = "layui-form-danger",
        verifyElem = $(".layui-form").find("*[lay-verify]");

    layui.each(verifyElem, function (index, item) {
        if (!$(this).is(":visible") && this.type !== "select-one") return false;
        var othis = $(this), vers = othis.attr("lay-verify"), tips = "", vType = othis[0].getAttribute("lay-verType");
        var list = (vers || "").split("|");
        for (var i = 0; i < list.length; i++) {
            var ver = list[i];
            if (!ver) continue;
            var value = othis.val(), isFn = typeof verify[ver] === "function";
            othis.removeClass(danger);
            // 如果是正则表达式且value为空则不验证
            if (ver !== "required" && value === "" && !isFn) continue;
            if (verify[ver] && (isFn ? tips = verify[ver](value, item) : !verify[ver][0].test(value))) {
                var msg = tips || verify[ver][1];
                if (vType == "tip") {
                    var activeItem = item;
                    if (item.tagName === "SELECT") {
                        activeItem = $(item).next();
                    }
                    layer.tips(msg, activeItem, {
                        time: 5000,
                        tips: 3
                    });
                } else if (vType == "alert") {
                    layer.alert(msg, {
                        title: '提示',
                        icon: 5
                    });
                } else {
                    layer.msg(msg, {
                        icon: 5,
                        shift: 6
                    });
                }
                //非移动设备自动定位焦点
                var device = layui.device();
                if (!device.android && !device.ios) {
                    item.focus();
                }
                othis.addClass(danger);
                result = false;
                // 返回true则表示结束循环
                return true;
            }
        }

        return false;
    });
    return result;
};

App.prototype.openSocket = function (userId, callback) {
    if (this.isBlank(userId)) return;
    if (callback) {
        this.imCallbacks.push(callback);
    }
    if (!this.socket) {
        this.socket = io.connect(this.imHost,
            {
                'force new connection': true,
                'connect timeout': 2000,
                'reconnect': false
            });
        this.socket.removeAllListeners();
    }

    // 监听发送给自己的数据
    this.socket.on('to' + userId,
        function (data) {
            console.log(JSON.stringify(data));
            for (var i in app.imCallbacks) {
                app.imCallbacks[i](data);
            }
        });

    // 监听公共数据
    this.socket.on('toall',
        function (data) {
            console.log(JSON.stringify(data));
            for (var i in app.imCallbacks) {
                app.imCallbacks[i](data);
            }
        });

    this.socket.on('connect',
        function (data) {
            console.log('socket connected');
            //netError.classList['add']('mui-hidden');
        });

    this.socket.on('disconnect',
        function (data) {
            console.log('socket disconnected');
        });

    // 连接上服务器时触发
    this.socket.on('online',
        function () {
            console.log('online');
        });
};

App.prototype.initPage = function (controlId, currPage, count, pageSize, callback) {
    var option = {
        elem: controlId,
        count: count,
        limit: pageSize,
        curr: currPage,
        limits: [5, 10, 20, 50, 100],
        layout: ['count', 'prev', 'page', 'next', 'skip', 'limit'],
        jump: function (obj, first) {
            if (!first && callback) {
                callback(obj.curr, obj.limit);
            }
        }
    };
    if (count <= pageSize) {
        option.theme = "hidden";
    }
    layui.laypage.render(option);
};

App.prototype.loadList = function (url, condition, finishCallback, controlId) {
    var numberPerPage = 10;
    condition = condition || { page: 1 };
    var self = this;
    self.invokeService({
        url: url,
        data: condition,
        success: function (result) {
            var pageSize = condition.pageSize || numberPerPage;
            var count = result.Count;
            var pageCount = Math.ceil(count / pageSize);
            if (condition.count !== count) {
                condition.page = Math.min(Math.max(pageCount, 1), condition.page);
                self.initPage(controlId, condition.page, count, pageSize, function (p, pageSize) {
                    condition.page = p;
                    condition.pageSize = pageSize;
                    self.loadList(url, condition, finishCallback, controlId);
                });
                condition.count = count;
            }
            if (finishCallback) {
                finishCallback(result.Data);
            }
        }
    });
};

App.prototype.openWindow = function (option) {
    var width = option.width || parseInt(window.screen.width * 0.75);
    var height = option.height || parseInt(window.screen.height * 0.75);
    var title = option.title || "";
    var url = option.url || "";
    var showOnTop = option.showOnTop || false;
    var btn = option.btn || [];
    var callback = option.callback || [];
    var shadeClose = option.shadeClose || false;
    if (typeof (option.closeBtn) == "undefined") {
        option.closeBtn = true;
    }

    var self = this;
    var windowOption = {
        type: 2,
        area: [width + "px", height + "px"],
        title: title,
        content: url,
        btn: btn,
        shadeClose: shadeClose,
        closeBtn: option.closeBtn ? 1 : 0
    };
    if (option.success) {
        windowOption.success = function (la, index) {
            var contentWindow = showOnTop ? top.window[la.find("iframe")[0]["name"]] : window[la.find("iframe")[0]["name"]];
            option.success(contentWindow, index);
        };
    }
    if (option.cancel) {
        windowOption.cancel = function (index, la) {
            var contentWindow = showOnTop ? top.window[la.find("iframe")[0]["name"]] : window[la.find("iframe")[0]["name"]];
            option.cancel(contentWindow, index);
        };
    }

    var addFunc = function (option, funcName, func, showOnTop) {
        option[funcName] = function (index, la) {
            var contentWindow = showOnTop ? top.window[la.find("iframe")[0]["name"]] : window[la.find("iframe")[0]["name"]];
            self.startLoading();
            func(contentWindow, index);
            self.endLoading();
            return false;
        };
    };
    for (var i = 0; i <= callback.length - 1; i++) {
        var funcName = i === 0 ? "yes" : "btn" + (i + 1);
        addFunc(windowOption, funcName, callback[i], showOnTop);
    }
    if (showOnTop) {
        top.layer.open(windowOption);
    } else {
        layer.open(windowOption);
    }
};

App.prototype.closeWindow = function (index, showOnTop) {
    if (showOnTop) {
        top.layer.close(index);
    } else {
        layer.close(index);
    }
};

App.prototype.contains = function (targetArray, item) {
    for (var index = 0; index < targetArray.length; index++) {
        if (targetArray[index] == item) {
            return true;
        }
    }
    return false;
};

App.prototype.distinct = function (targetArray) {
    var result = [];
    for (var index = 0; index < targetArray.length; index++) {
        if (this.contains(result, targetArray[index])) {
            continue;
        }
        result.push(targetArray[index]);
    }
    return result;
};

App.prototype.intersection = function (array1, array2) {
    var result = [];
    for (var i = 0; i < array1.length; i++) {
        if (this.contains(array2, array1[i])) {
            result.push(array1[i]);
        }
    }
    return this.distinct(result);
};

var app = new App();

layui.form.verify({
    mobile: [/^1[345789][0-9]\d{8}$/, "手机号格式不正确"],
    floadOrInt: [/^\d{1,7}(?:\.\d{0,2}$|$)/, "请输入小数或整数"],
    email: [/^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/, "Email格式不正确"],
    idCard: [/^([0-9]){7,18}(x|X)?$/, "身份证号码格式不正确"],
    date: [/^\d{4}-\d{1,2}-\d{1,2}/, "日期格式格式不正确"],
    number_all: [/^(\-|\+)?\d+(\.\d+)?$/, "请输入数字"],
    password: [/^[a-zA-Z]\w{5,11}/, "密码必须以字母开头，由6-12位字母、数字或下划线组成"],
    chineseName: [/^([\u4e00-\u9fa5\·]{1,10})$/, "请输入正确的中文名"],
    nonnegative: [/^\d+(\.{0,1}\d+){0,1}$/, "请输入非负的数值"],
    positiveNumber: [/^[+]{0,1}(\d+)$|^[+]{0,1}(\d+\.\d+)$/, "请输入正数"]
});