/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
; (function ($) {

    document.oncontextmenu = function () { return false; }

    //移除
    $.extend(Array.prototype, {
        remove: function (dx, to) {
            this.splice(dx, (to || 1));
        }
    });

    //日期格扩展
    $.extend(Date.prototype, {
        format: function (fmt) {
            var date = this;
            var o = {
                "M+": date.getMonth() + 1,
                "d+": date.getDate(),
                "h+": date.getHours(),
                "m+": date.getMinutes(),
                "s+": date.getSeconds(),
                "q+": Math.floor((date.getMonth() + 3) / 3),
                "S": date.getMilliseconds()
            };

            if (/(y+)/.test(fmt)) {
                fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
            }

            for (var k in o) {
                if (new RegExp("(" + k + ")").test(fmt)) {
                    fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) :
                          (("00" + o[k]).substr(("" + o[k]).length)));
                }
            }
            return fmt;
        }
    });

    //继承
    $.extend(Function.prototype, {
        extend: function (Parent, Override) {
            function F() { }
            F.prototype = Parent.prototype;
            this.prototype = new F();
            this.prototype.constructor = this;
            this.base = {};
            this.base.Parent = Parent;
            this.base.Constructor = Parent;
            if (Override) {
                $.extend(this.prototype, Override);
            }
        }
    });

    //占位符格式化
    $.extend(String.prototype, {
        format: function () {
            if (arguments.length == 0)
                return '';
            var str = arguments[0];
            for (var i = 1; i < arguments.length; i++) {
                var re = new RegExp('\\\\{' + (i - 1) + '\\\\}', 'gm');
                str = str.replace(re, arguments[i]);
            }
            return str;
        }
    });


    function StringBuilder() {
        this.elements = [];
    }

    StringBuilder.prototype = {
        constructor: StringBuilder,
        append: function (text) {
            this.elements.push(text);
            return this;
        },
        toString: function () {
            return this.elements.join('');
        }
    }

    window.util = {
        ie: (!!window.ActiveXObject || "ActiveXObject" in window),
        parseNode: function (layout) {
            var pos = layout.split(' ');
            return {
                x: Number(pos[0]),
                y: Number(pos[2]),
                disX: Number(pos[1]),
                disY: Number(pos[3])
            };
        },
        parseLine: function (layout) {
            var pos = layout.split(' ');
            return {
                x1: Number(pos[0]),
                y1: Number(pos[1]),
                x2: Number(pos[2]),
                y2: Number(pos[3])
            };
        },
        builder: function () {
            return new StringBuilder();
        }
    };

})(jQuery);