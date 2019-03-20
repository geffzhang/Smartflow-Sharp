/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
(function () {
    var
        roleGridSelector = '#roleGrid',
        assignToRoleSelector = '#roleAssign',
        cmdTextSelector = '#txtCommand',
        optionSelector = '#ddlRuleConfig option:selected',
        ruleSelector = '#ddlRuleConfig',
        itemTemplate = "<li id=%{0}%>%{1}%</li>",
        lineTemplate = "<tr><td class='layui-text smartflow-header'>%{0}%</td><td><input type='text' value=\"%{1}%\" id=%{2}% class='layui-input smartflow-input' /></td></tr>";
    tabConfig = {
        node: ['#tab li[category=rule]', '#tab li[category=form]'],
        decision: ['#tab li[category=role]', '#tab li[category=form]'],
        start: ['#tab li[category=rule]', '#tab li[category=role]', '#tab li[category=info]']
    },
    config = {
        //开始
        start: '<',
        //结束
        end: '>',
        //左引号
        lQuotation: '"',
        //右引号
        rQuotation: '"',
        //闭合
        beforeClose: '</',
        //闭合
        afterClose: '/>',
        //等于
        equal: '=',
        //本身引用
        //空隙
        space: ' ',
    };


    var CONST_ROLE_FIELD_MAP= {
        ID: 'IDENTIFICATION',
        NAME: 'APPELLATION'
    };
    var CONST_CONFIG_FIELD_MAP = {
        ID: 'ID',
        NAME: 'NAME',
        CONNECTIONSTRING: 'CONNECTIONSTRING',
        PROVIDERNAME: 'PROVIDERNAME'
    };
 

    $.extend(String.prototype, {
        format: function () {
            var regexp = /\{(\d+)\}/g;
            var args = arguments,
                escapeChar = '';
            var result = this.replace(regexp, function (m, i, o, n) {
                return args[i];
            });
            return result.replaceAll('%', escapeChar);
        },
        replaceAll: function (searchValue, replaceValue) {
            var regExp = new RegExp(searchValue, "g");
            return this.replace(regExp, replaceValue);
        }
    });

    function initOption(option) {
        config = $.extend(config, option);
    }

    function loadRoleGrid(group, key) {

        var ajaxSettings = { url: config.roleUrl };
        if (group.length > 0) {

            var roleIds = [];
            $.each(group, function () {
                roleIds.push(this.id);
            });

            ajaxSettings.data = {
                roleIds: roleIds.join(',')
            };
        }

        ajaxSettings.data = ajaxSettings.data || {};
        ajaxSettings.data.searchKey = key;

        ajaxSettings.success = function (serverData) {
            var build = util.builder(),
                Abuild = util.builder();
            $.each(serverData, function () {
                build.append(config.start)
                    .append('li')
                    .append(config.space)
                    .append('id')
                    .append(config.equal)
                    .append(config.lQuotation)
                    .append(this[CONST_ROLE_FIELD_MAP.ID])
                    .append(config.rQuotation)
                    .append(config.space)
                    .append('name')
                    .append(config.equal)
                    .append(config.lQuotation)
                    .append(this[CONST_ROLE_FIELD_MAP.NAME])
                    .append(config.rQuotation)
                    .append(config.end)
                    .append(this[CONST_ROLE_FIELD_MAP.NAME])
                    .append(config.beforeClose)
                    .append('li')
                    .append(config.afterClose);
            });

            $(roleGridSelector).html(build.toString());

            $(roleGridSelector).on("dblclick", "li", function () {
                $(assignToRoleSelector).append(this);
            });

            $(assignToRoleSelector).on("dblclick", "li", function () {
                $(roleGridSelector).append(this);
            });

            $.each(group, function () {
                Abuild.append(config.start)
                    .append('li')
                    .append(config.space)
                    .append('id')
                    .append(config.equal)
                    .append(config.lQuotation)
                    .append(this.id)
                    .append(config.rQuotation)
                    .append(config.space)
                    .append('name')
                    .append(config.equal)
                    .append(config.lQuotation)
                    .append(this.name)
                    .append(config.rQuotation)
                    .append(config.end)
                    .append(this.name)
                    .append(config.beforeClose)
                    .append('li')
                    .append(config.afterClose);
            });

            $(assignToRoleSelector).html(Abuild.toString());
        };
        ajaxService(ajaxSettings);
    }

    function loadForm(form) {
        if (form) {
            $("#txtElement").val(form.text);
        }
    }

    function setSettingsToNode(nx) {
        var roles = [],
            expressions = [],
            name = $("#txtNodeName").val();
        if (nx.category.toLowerCase() === 'decision') {
            $("#transitions tbody input").each(function () {
                var input = $(this);
                expressions.push({ id: input.attr("id"), expression: input.val() });
            });

            var cmdText = $(cmdTextSelector).val(),
                sourceID = $(optionSelector).val();
            if (cmdText != '' && cmdText) {
                nx.command = {
                    id: sourceID,
                    text: cmdText
                };
            }
            nx.setExpression(expressions);
        } else {
            $("#roleAssign li").each(function () {
                var self = $(this);
                roles.push({ id: self.attr("id"), name: self.attr("name") });
            });

            var text = $("#txtElement").val();
            nx.form = {
                text: text,
                name: '业务表单'
            };
            nx.group = roles;
        }

        if (name) {
            nx.name = name;
            nx.brush.text(nx.name);
        }
    }

    function setNodeToSettings(nx) {
        $("#txtNodeName").val(nx.name);
        if (nx.category.toLowerCase() === 'decision') {
            var lineCollection = nx.getTransitions();
            if (lineCollection.length > 0) {
                var unqiueId = 'lineTo',
                    build = util.builder();
                $.each(lineCollection, function (i) {
                    build.append(lineTemplate.format(this.name, this.expression, this.id));
                });
                $("#transitions>tbody").html(build.toString());
            }
            loadSelect(nx.command);
        } else {
            loadRoleGrid(nx.group);
            loadForm(nx.form);
        }

        var nodeName = nx.category.toLocaleLowerCase(),
            items = tabConfig[nodeName];
        $.each(items, function (i, selector) {
            $(selector).hide();
        });

        if (nodeName === "start") {
            $('#tab li[category=form]').trigger('click');
        }
    }

    function loadSelect(command) {
        var settings = {
            url: config.configUrl
        };
        settings.success = function (serverData) {
            var build =util.builder();
            $.each(serverData, function () {
                var data = JSON.stringify(this);

                build.append(config.start)
                     .append('option')
                     .append(config.space)
                     .append('data')
                     .append(config.equal)
                     .append(config.lQuotation)
                     .append(escape(data))
                     .append(config.rQuotation)
                     .append(config.space)
                     .append('value')
                     .append(config.equal)
                     .append(config.lQuotation)
                     .append(this[CONST_CONFIG_FIELD_MAP.ID])
                     .append(config.rQuotation)
                     .append(config.end)

                     .append(this[CONST_CONFIG_FIELD_MAP.NAME])

                     .append(config.beforeClose)
                     .append('option')
                     .append(config.afterClose);
            });
            $(ruleSelector).html(build.toString());

            if (command) {
                $(cmdTextSelector).val(command.text);
                $(ruleSelector).val(command.id);
            }
        }
        ajaxService(settings);
    }

    function ajaxService(settings) {
        var defaultSettings = $.extend({
            dataType: 'json',
            type: 'post',
            cache: false
        }, settings);
        $.ajax(defaultSettings);
    }

    function doSearch(searchKey) {
        var roles = [];
        $("#roleAssign li").each(function () {
            var self = $(this);
            roles.push({ id: self.attr("id"), name: self.attr("name") });
        });
        loadRoleGrid(roles, searchKey);
    }

    window.SMF = {
        init: initOption,
        search: doSearch,
        setNodeToSettings: setNodeToSettings,
        setSettingsToNode: setSettingsToNode
    };
})();