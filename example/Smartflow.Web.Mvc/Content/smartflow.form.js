/************************************************************************************
 * 描述：公共表单业务工具类
 * 作者：smartflow-sharp
 * 日期：2019.3.5
 * **********************************************************************************/
; (function () {

    /**
     * 检查值是否为空
     * @param {any} value
     */
    smart.isNull = function (value) {
        return (!(value == '' || value == undefined))
    }

    /**
     * 发送ajaxPost请求
     * @param {any} settings
     */
    smart.ajaxPost = function (settings) {
        var defaultSettings = $.extend({
            type: 'post',
            cache: false,
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded'
        }, settings);

        defaultSettings.url = (smart.path + defaultSettings.url);

        $.ajax(defaultSettings);
    }
    /**
     * 加载
     * @param {any} instanceID
     */
    smart.load = function (instanceID) {
        if (this.isNull(instanceID)) {
            smart.ajaxPost({
                url: 'WebView/GetWebView',
                data: { relation: smart.env.config, instanceID: instanceID },
                success: function (data) {
                    if (smart.loadRecord) {
                        smart.loadRecord.call(this, data);
                    } else {
                        alert("loadRecord");
                    }
                }
            })
        }
    }

    smart.start = function () {
        if ($.isFunction(smart.beforeStart)) {
            var form = smart.beforeStart.call(window);
            smart.ajaxPost({
                url: 'WebView/SaveWebView',
                data: {
                    relation: JSON.stringify(smart.env.config),
                    form: JSON.stringify(form)
                },
                success: function () {
                    window.close();
                }
            })
        } else {
            alert('请订阅beforeSubmit事件');
        }
    }
})();