/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: https://www.smartflow-sharp.com
 ********************************************************************
 */
(function ($) {

    //var CONST_ATTRIBUTE_FIELD_MAP = { id: 'id', name: 'name', from: 'origin', to: 'destination' };
    //var CONST_ACTION_TIP = [
    //    { title: '审核人：', fieldName: 'Name', format: function (value) { return value; } },
    //    { title: '时间：', fieldName: 'CreateDateTime', format: function (value) { return new Date(value).format('yyyy-MM-dd hh:mm'); } },
    //    {
    //        title: '操作：', fieldName: 'Operation', format: function (value)
    //        {
    //            var ACTION_VALUE_MAP = { 0: '审核', 1: '流程撤销', 2: '流程退回' };
    //            return ACTION_VALUE_MAP[value];
    //        }
    //}];

    var NC = {},
        LC = {},
        RC = [],
        draw,
        fromConnect,
        drawOption;

    var config = {
        rootStart: '<workflow>',
        rootEnd: '</workflow>',
        start: '<',
        end: '>',
        lQuotation: '"',
        rQuotation: '"',
        beforeClose: '</',
        afterClose: '/>',
        equal: '=',
        space: ' ',
        group: 'group',
        form:'form',
        from: 'from',
        actor: 'actor',
        transition: 'transition',
        br: 'br',
        id: 'id',
        name: 'name',
        to:'destination'
     };

    function init(option) {
        draw = SVG(option.container);
        draw.mouseup(function (e) {
            draw.off('mousemove');
        });

        drawOption = $.extend(drawOption, option);
        unselect();
        return draw;
    }

    function unselect() {
        $(document).bind('mousedown', function () { return false; });
        $(document).bind('mouseup', function () { return false; });
    }

    function Element(name, category) {
        //节点ID
        this.id = undefined;
        //文本画笔
        this.brush = undefined;
        //节点中文名称
        this.name = name;
        //节点类别（LINE、NODE、START、END,DECISION）
        this.category = category;
        this.unique = undefined;
        //禁用事件
        this.disable = false;
        //背景颜色
        this.bgColor = '#f06';
        //当前节点颜色
        this.bgCurrentColor = '#6DEA47';
    }

    Element.prototype = {
        constructor: Element,
        draw: function () {
            if (!this.disable) {
                this.bindEvent.apply(SVG.get(this.id), [this]);
            }
            return this.id;
        },
        bindEvent: function (o) {
            var className = this.type;
            this.addClass(className);
        }
    };

    function Line() {
        this.x1 = 0;
        this.y1 = 0;
        this.x2 = 0;
        this.y2 = 0;
        this.border = 3;
        this.orientation = 'down';
        this.expression = '';
        Line.base.Constructor.call(this, "line", "line");
    }

    Line.extend(Element, {
        constructor: Line,
        draw: function () {
            var self = this,
                l = draw.line(self.x1, self.y1, self.x2, self.y2);
            l.stroke({ width: self.border, color: self.bgColor });
            self.brush = draw.text(self.name);

            l.marker('end', 10, 10, function (add) {
                add.path('M0,0 L0,6 L6,3 z').fill("#f00");
                this.attr({ refX: 5, refY: 2.9, orient: 'auto', stroke: 'none', markerUNits: 'strokeWidth' });
            });

            self.brush.attr({ x: (self.x2 - self.x1) / 2 + self.x1, y: (self.y2 - self.y1) / 2 + self.y1 });
            self.id = l.id();
            LC[self.id] = this;
            return Line.base.Parent.prototype.draw.call(self);
        },
        bindEvent: function (l) {
            this.dblclick(function (evt) {
                evt.preventDefault();
                var instance = LC[this.id()];
                if (evt.ctrlKey && evt.altKey) {
                    eachElements(instance.id);
                    this.off('dblclick');
                    this.remove();
                    instance.brush.remove();
                    delete LC[instance.id];
                } else {
                    var nodeName = prompt("请输入路线名称", instance.name);
                    if (nodeName) {
                        instance.name = nodeName;
                        instance.brush.text(instance.name);
                    }
                }
                return false;
            });
            Line.base.Parent.prototype.bindEvent.call(this, l);
        }
    });

    function Node() {
        this.w = 180;
        this.h = 40;
        this.x = 10;
        this.y = 10;
        this.cx = 40;
        this.cy = 10;
        this.disX = 0;
        this.disY = 0;
        this.group = [];
        this.form = undefined;
        this.actors = [];
        Node.base.Constructor.call(this, "node", "node");
        this.name = "节点";
    }

    Node.extend(Element, {
        getTransitions: function () {
            var elements = findByElementId(this.id, config.from),
                lineCollection = [];
            $.each(elements, function () {
                lineCollection.push(LC[this.id]);
            });
            return lineCollection;
        },
        setExpression: function (expressions) {
            $.each(expressions, function () {
                LC[this.id].expression = this.expression;
            });
        },
        draw: function (b) {
            var n = this,
                color = (b == n.unique && b && n.unique) ? n.bgCurrentColor : n.bgColor,
                rect = draw.rect(n.w, n.h).attr({ fill: color, x: n.x, y: n.y });

            n.brush = draw.text(n.name);
            n.brush.attr({ x: n.x + rect.width() / 2, y: n.y + rect.height() / 2 + n.vertical() });

            n.id = rect.id();
            NC[n.id] = n;
            return Node.base.Parent.prototype.draw.call(this);
        },
        checkRule: function (nf) {
            return ((nf.category === 'end' || this.category === 'start') ||(nf.category === 'start' && this.category === 'end'));
        },
        bindEvent: function (n) {
            this.mousedown(OnDrag);
            this.dblclick(function (evt) {
                evt.preventDefault();
                var node = NC[this.id()];
                node.edit.call(this, evt);
                return false;
            });
            Node.base.Parent.prototype.bindEvent.call(this, n);
        },
        edit: function (evt) {
            if (evt.ctrlKey && evt.altKey) {
                var id = this.id(),
                    node = NC[id],
                    rect = SVG.get(id),
                    elements = findByElementId(id);

                deleteElement(elements);

                rect.remove();
                node.brush.remove();
                delete NC[id];

            } else {
                var nx = NC[this.id()];
                drawOption['dblClick']
                            && drawOption['dblClick'].call(this, nx);
            }
        },
        move: function (element, d) {
            var self = this;
            self.x = d.clientX - self.disX - self.cx;
            self.y = d.clientY - self.disY - self.cy;
            element.attr({ x: self.x, y: self.y });

            if (self.brush) {
                self.brush.attr({ x: (element.x() + (element.width() / 2)), y: element.y() + (element.height() / 2) + self.vertical() });
            }

            var toElements = findByElementId(self.id, "to"),
                fromElements = findByElementId(self.id, "from");

            $.each(toElements, function () {
                var lineElement = SVG.get(this.id),
                    instance = LC[this.id];

                if (lineElement && instance) {
                    instance.x2 = self.x + this.ox2;
                    instance.y2 = self.y + this.oy2;
                    lineElement.attr({ x2: instance.x2, y2: instance.y2 });
                    instance.brush.attr({ x: (instance.x2 - instance.x1) / 2 + instance.x1, y: (instance.y2 - instance.y1) / 2 + instance.y1 });
                }
            });

            $.each(fromElements, function () {
                var lineElement = SVG.get(this.id),
                    instance = LC[this.id];
                if (lineElement && instance) {
                    instance.x1 = self.x + this.ox1;
                    instance.y1 = self.y + this.oy1;
                    lineElement.attr({ x1: instance.x1, y1: instance.y1 });
                    instance.brush.attr({ x: (instance.x2 - instance.x1) / 2 + instance.x1, y: (instance.y2 - instance.y1) / 2 + instance.y1 });
                }
            });

            setTimeout(function () {
                //判断ie是否支持
                if (util.ie) {
                    var svn = document.getElementById(drawOption.container);
                    svn.insertAdjacentElement("beforeEnd", svn.firstElementChild);
                }
            }, 0);
        },
        exportElement: function () {
            var
                self = this,
                build = util.builder();

            build.append(config.start)
                 .append(self.category)
                 .append(config.space)
                 .append("id")
                 .append(config.equal)
                 .append(config.lQuotation)
                 .append(self['unique'])
                 .append(config.rQuotation)
                 .append(config.space)
                 .append(config.name)
                 .append(config.equal)
                 .append(config.lQuotation)
                 .append(self[config.name])
                 .append(config.rQuotation)
                 .append(config.space)
                 .append('layout')
                 .append(config.equal)
                 .append(config.lQuotation)
                 .append(self.x + ' ' + self.disX + ' ' + self.y + ' ' + self.disY)
                 .append(config.rQuotation)
                 .append(config.end);

            $.each(self.group, function () {
                build.append(config.start)
                     .append(config.group);
                eachAttributs(build, this);
                build.append(config.afterClose);
            });

            if (self.form) {
                var formElement = self.form;
                build.append(config.start)
                    .append(config.form)
                    .append(config.space)
                    .append(config.name)
                    .append(config.equal)
                    .append(config.lQuotation)
                    .append(formElement[config.name])
                    .append(config.rQuotation)
                    .append(config.end)
                    .append("<![CDATA[")
                    .append(formElement.text)
                    .append("]]>")
                    .append(config.beforeClose)
                    .append(config.form)
                    .append(config.end);
            }

            if (self.exportDecision) {
                self.exportDecision(build);
            }

            var elements = findByElementId(self.id, config.from);
            $.each(elements, function () {
                if (this.from === self.id) {
                    var
                        L = LC[this.id],
                        N = NC[this.to];

                    build.append(config.start)
                        .append(config.transition)
                        .append(config.space)
                        .append(config.name)
                        .append(config.equal)
                        .append(config.lQuotation)
                        .append(L.name)
                        .append(config.rQuotation)
                        .append(config.space)
                        .append(config.to)
                        .append(config.equal)
                        .append(config.lQuotation)
                        .append(N.unique)
                        .append(config.rQuotation)
                        .append(config.space)
                        .append('layout')
                        .append(config.equal)
                        .append(config.lQuotation)
                        .append(L.x1 + ' ' + L.y1 + ' ' + L.x2 + ' ' + L.y2)
                        .append(config.rQuotation)
                        .append(config.end);

                    if (self.category === 'decision') {

                        build.append("<![CDATA[")
                             .append(L.expression)
                             .append("]]>");
                    } 

                    build.append(config.beforeClose)
                         .append(config.transition)
                         .append(config.end);
                }
            });

            $.each(self.actors, function () {
                build.append(config.start)
                    .append(config.actor);
                eachAttributs(build, this);
                build.append(config.afterClose);
            });

            build.append(config.beforeClose)
                 .append(self.category)
                 .append(config.end);

          
            function eachAttributs(build, reference) {
                $.each(['id', 'name'], function (i, p) {
                    build.append(config.space)
                        .append(config[p])
                        .append(config.equal)
                        .append(config.lQuotation)
                        .append(reference[p])
                        .append(config.rQuotation);
                });
            }

            return build.toString();
        },
        validate: function () {
            return (findByElementId(this.id, 'to').length > 0
                   && findByElementId(this.id, 'from').length > 0);
        },
        vertical: function () {
            return util.ie ? 6 : 0;
        }
    });

    Node.prototype.showToolTip = function (data) {
        var n = this,
            rect = SVG.get(n.id),
            tooltip = draw.element('title'),
            tn = tooltip.node,
            fragmeng = document.createDocumentFragment();

        $.each(data, function () {
            var serverData = this;
            $.each(CONST_ACTION_TIP, function () {
                var column = this.title + this.format(serverData[this.fieldName]);
                fragmeng.appendChild(document.createTextNode(column));
                fragmeng.appendChild(document.createElement(config.br));
            });
        });
        tn.appendChild(fragmeng);
        rect.node.appendChild(tn);
    }

    Node.prototype.revert = function (layout, currentNodeID) {
        $.extend(this, util.parseNode(layout));
        this.draw(currentNodeID);
        $.each(this.transitions, function () {
            var instance = new Line();
            $.extend(instance, this, util.parseLine(this.layout));

            instance.disable = (disable || false);
            instance.draw();
        });
    }

    function Decision() {
        Decision.base.Constructor.call(this);
        this.name = '分支节点';
        this.category = 'decision';
        this.circles = [];
        this.command = undefined;
    }

    Decision.extend(Node, {
        draw: function () {
            Decision.base.Parent.prototype.draw.call(this);
            var y = this.y + this.h,
                w = this.w;

            for (var i = 0, len = w / 20; i < len; i++) {
                var circle = draw.circle(20);
                circle.attr({ fill: '#F485B2', cx: this.x + i * 20 + 10, cy: y });
                circle.addClass('circle');

                var rect = draw.rect(20, 20).attr({
                    x: this.x + i * 20,
                    y: y - 20
                });
                var clip = draw.clip().add(rect);
                circle.clipWith(clip);

                circle.attr({ decisionId: this.id });
                this.circles.push(circle);
            }
        },
        edit: function (evt) {
            if (evt.ctrlKey && evt.altKey) {
                var decision = NC[this.id()];
                $.each(decision.circles, function () {
                    this.remove();
                });
            }
            Decision.base.Parent.prototype.edit.call(this, evt);
        },
        move: function (element, evt) {
            Decision.base.Parent.prototype.move.call(this, element, evt);
            var self = this, y = self.y + self.h;
            $.each(self.circles, function (i) {
                var clipRect = this.reference('clip-path');
                var rect = SVG.get(clipRect.node.firstChild.id);
                rect.attr({ x: self.x + i * 20, y: y - 20 });
                this.attr({ fill: '#F485B2', cx: self.x + i * 20 + 10, cy: y });
            });
        },
        bindEvent: function (decision) {
            Decision.base.Parent.prototype.bindEvent.call(this, decision);
        },
        exportDecision: function (build) {
            var self = this;
            if (self.command) {

                build.append(config.start)
                     .append('command')
                     .append(config.end);

                $.each(self.command, function (propertyName, value) {
                    build.append(config.start)
                         .append(propertyName)
                         .append(config.end)
                         .append("<![CDATA[")
                         .append(value)
                         .append("]]>")
                         //.append(value)
                         .append(config.beforeClose)
                         .append(propertyName)
                         .append(config.end);
                });

                build.append(config.beforeClose)
                     .append('command')
                     .append(config.end);
            }
        },
        validate: function () {
            return (findByElementId(this.id, 'from').length > 1
                 && findByElementId(this.id, 'to').length > 0);
        }
    });

    function Start() {
        Start.base.Constructor.call(this);
        this.category = "start";
        this.name = "开始";
    }

    Start.extend(Node, {
        draw: function () {
            Start.base.Parent.prototype.draw.call(this);
            var nid = this.id,
                rect = SVG.get(nid);

            rect.radius(10);
        },
        bindEvent: function (n) {
            Start.base.Parent.prototype.bindEvent.call(this, n);
            //this.off('dblclick');
        },
        validate: function () {
            return (findByElementId(this.id, 'from').length > 0
                   && findByElementId(this.id, 'to').length == 0);
        }
    });

    function End() {
        End.base.Constructor.call(this);
        this.category = "end";
        this.name = "结束";
    }

    End.extend(Node, {
        constructor: End,
        draw: function () {
            End.base.Parent.prototype.draw.call(this);
            var nid = this.id,
                rect = SVG.get(nid);

            rect.radius(10);
        },
        bindEvent: function (n) {
            End.base.Parent.prototype.bindEvent.call(this, n);
            this.off('dblclick');
        },
        validate: function () {
            return (findByElementId(this.id, 'from').length == 0
                   && findByElementId(this.id, 'to').length > 0);
        }
    });

    function select() {
        initEvent();
        draw.each(function (i, child) {
            if (this.type === 'rect') {
                this.mousedown(OnDrag);
            }
        });
    }

    function alignElement() {
        var sn;
        for (var p in NC) {
            if (NC[p].category == 'start') {
                sn = NC[p];
                break;
            }
        }

        if (!sn) return;

        var ele = SVG.get(sn.id);
        $.each(NC, function () {
            if (this.category != 'start') {
                var element = SVG.get(this.id);
                this.disX = sn.disX;
                this.disY = sn.disY;

                this.move.call(this, element, {
                    clientX: ele.x() + this.cx + this.disX,
                    clientY: element.y() + this.cy + this.disY
                });
            }
        });
    }

    function connect() {
        initEvent();
        $(document).bind('mousedown', OnConnect);
        $(document).bind('mouseup', OnConnected);
    }

    function OnConnect(evt) {
        var node = $(evt.target).get(0),
            nodeName = node.nodeName,
            nodeId = node.id;

        if (nodeName === 'rect' || nodeName === 'circle') {
            var instance, y, x;
            if (!NC[nodeId]) {
                var decisionId = node.getAttribute("decisionId");
                instance = NC[decisionId];
                y = node.instance.cy();
                x = node.instance.cx();
            } else {
                instance = NC[nodeId];
                y = evt.clientY - instance.cy;
                x = evt.clientX - instance.cx;
            }

            fromConnect = {
                id: instance.id,
                x: x,
                y: y
            }
        }

        return false;
    }

    function OnConnected(evt) {
        var node = $(evt.target).get(0),
            nodeName = node.nodeName,
            nodeId = node.id;

        if (nodeName === 'rect' && fromConnect) {

            var toRect = SVG.get(nodeId),
                fromRect = SVG.get(fromConnect.id),
                nt = NC[nodeId],
                nf = NC[fromConnect.id];

            var duplicateCheck = function (from, to) {
                var result = false;
                for (var i = 0, len = RC.length; i < len; i++) {
                    var r = RC[i];
                    if (r.from === from && r.to === to) {
                        result = true;
                        break;
                    }
                }
                return result;
            };

            if (nodeId !== fromConnect.id
                && !nt.checkRule(nf)
                && !duplicateCheck(fromConnect.id, nodeId)) {

                var checkOrientation = function (from, to) {
                    var orientation = 'down';
                    if (from.y() < to.y()) {
                        orientation = 'down';
                    } else {
                        orientation = 'up'
                    }
                    return orientation;
                }

                var instance = new Line(),
                    orientation = checkOrientation(fromRect, toRect);

                if (orientation === 'down' && nf.category === 'decision') {
                    instance.x1 = fromConnect.x;
                    instance.y1 = fromRect.height() + fromRect.y();
                    instance.x2 = toRect.width() / 2 + toRect.x();
                    instance.y2 = toRect.y();
                }
                else if (orientation === 'down') {
                    instance.x1 = fromRect.width() / 2 + fromRect.x();
                    instance.y1 = fromRect.height() + fromRect.y();
                    instance.x2 = toRect.width() / 2 + toRect.x();
                    instance.y2 = toRect.y();

                } else {
                    instance.x1 = fromConnect.x;
                    instance.y1 = fromRect.y();
                    instance.x2 = evt.clientX - nt.cx;
                    instance.y2 = toRect.height() + toRect.y();
                }

                instance.orientation = orientation;
                instance.draw();

                var l = SVG.get(instance.id),
                    r = SVG.get(fromConnect.id);

                RC.push({
                    id: instance.id,
                    from: fromConnect.id,
                    to: nodeId,
                    ox2: l.attr("x2") - toRect.x(),
                    oy2: l.attr("y2") - toRect.y(),
                    ox1: l.attr("x1") - fromRect.x(),
                    oy1: l.attr("y1") - fromRect.y()
                });
            }
        }
        fromConnect = undefined;
        evt.preventDefault();
        return false;
    }

    function initEvent() {
        $(document).unbind('mousedown');
        $(document).unbind('mouseup');
        draw.each(function (i, child) {
            if (this.type === 'rect') {
                this.off('mousedown');
            }
        });
    }

    function OnDrag(evt) {
        var self = this,
            nx = NC[self.id()];
        evt.preventDefault();
        nx.disX = evt.clientX - self.x() - nx.cx;
        nx.disY = evt.clientY - self.y() - nx.cy;
        draw.on('mousemove', function (d) {
            d.preventDefault();
            nx.move(self, d);
            return false;
        });
        return false;
    }

    function deleteElement(elements) {
        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];
            if (element) {
                var l = SVG.get(element.id),
                    instance = LC[element.id];

                l.remove();
                instance.brush.remove();
                eachElements(element.id);
                delete LC[element.id];
            }
        }
    }

    function eachElements(id) {
        for (var i = 0, len = RC.length; i < len; i++) {
            if (RC[i].id == id) {
                RC.remove(i);
                break;
            }
        }
    }

    function findByElementId(elementId, propertyName) {
        var elements = [];
        $.each(RC, function () {
            var self = this;
            if (propertyName) {
                if (this[propertyName] === elementId) {
                    elements.push(this);
                }
            } else {
                $.each(["to", "from"], function () {
                    if (self[this] === elementId) {
                        elements.push(self);
                    }
                });
            }
        });
        return elements;
    }

    function exportToJSON() {
        var unique = 29,
            nodeCollection = [],
            pathCollection = [],
            validateCollection = [],
            build = util.builder();

        $.each(NC, function () {
            var self = this;
            if (!self.validate()) {
                validateCollection.push(false);
            }
        });

        if (validateCollection.length > 0 || (RC.length === 0)) {
            alert("流程图不符合流程定义规则");
            return;
        }

        function generatorId() {
            unique++;
            return unique;
        }

        for (var propertyName in NC) {
            NC[propertyName].unique = generatorId();
        }

        build.append(config.rootStart);
        $.each(NC, function () {
            var self = this;
            build.append(self.exportElement());
        });

        build.append(config.rootEnd);
        return { STRUCTUREXML: escape(build.toString()) };
    }

    function revert(data, disable, currentNodeId, process) {
        var record = process || [],
            findUID = function (destination) {
                var id;
                for (var i = 0, len = data.length; i < len; i++) {
                    var node = data[i];
                    if (destination == node.unique) {
                        id = node.id;
                        break;
                    }
                }
                return id;
            };
        $.each(data, function () {
            var self = this;
            this.category = (this.category.toLowerCase() == 'normal' ? 'node' : this.category.toLowerCase());
            var instance = convertToRealType(this.category);
            $.extend(instance, this, util.parseNode(self.layout));
            instance.disable = (disable || false);
            instance.draw(currentNodeId);
            self.id = instance.id;

            //遍历过程记录
            var setToolTipArray = function (data, id) {
                var toolArray = [];
                $.each(data, function () {
                    if (this.ID == id) {
                        toolArray.push(this);
                    }
                });
                return toolArray;
            }
            if (instance.disable && record.length > 0) {
                //instance.showToolTip(setToolTipArray(record, instance.unique));
            }
        });
        $.each(data, function () {
            var self = this;
            $.each(self.transitions, function () {
                var transition = new Line();
                $.extend(transition, this, util.parseLine(this.layout));
                transition.disable = (disable || false);
                transition.draw();

                var destinationId = findUID(transition.destination),
                    destination = SVG.get(destinationId),
                    from = SVG.get(self.id),
                    line = SVG.get(transition.id);

                RC.push({
                    id: transition.id,
                    from: self.id,
                    to: destinationId,
                    ox2: line.attr("x2") - destination.x(),
                    oy2: line.attr("y2") - destination.y(),
                    ox1: line.attr("x1") - from.x(),
                    oy1: line.attr("y1") - from.y()
                });
            });
        });
    }

    function convertToRealType(category) {
        switch (category) {
            case "node":
                convertType = new Node();
                break;
            case "start":
                convertType = new Start();
                break;
            case "end":
                convertType = new End();
                break;
            case "decision":
                convertType = new Decision();
                break;
            default:
                break;
        }
        return convertType;
    }

    //对外提供访问接口
    window.SMF = {
        init: init,
        select: select,
        connect: connect,
        //导出到JSON对象，以序列化保存到数据库
        exportToJSON: exportToJSON,
        revert: revert,
        alignment: alignElement,
        create: function (category) {
            var reallType = convertToRealType(category);
            reallType.x = Math.floor(Math.random() * 200 + 1);
            reallType.y = Math.floor(Math.random() * 200 + 1);
            reallType.draw();
        }
    };

})(jQuery);





