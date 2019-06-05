window.pieControls = {};
window.pieControls.arcs = {};
window.pieControls.color = d3.scaleOrdinal()
    .domain(['a', 'b'])
    .range(["#007bff", "#ffffff"]);

window.pieControls.angleC = 2.0 / 3.0;
window.pieControls.pie = d3.pie()
    .sort(null)
    .value(function (d) { return d.value; })
    .startAngle(-window.pieControls.angleC * Math.PI)
    .endAngle(window.pieControls.angleC * Math.PI);

window.pieControls.updateValue = function (ref, value) {
    const id = '#' + ref;
    var b = 1 - value;
    var data = { a: value, b: b };
    var data_ready = window.pieControls.pie(d3.entries(data));
    var arc = window.pieControls.arcs[id];
    var arcs = arc.arc
        .selectAll('path')
        .data(data_ready, function (d) { return d.data.key; });

    var arcGen = d3.arc()
        .innerRadius(arc.radius * 0.7)
        .outerRadius(arc.radius);

    arcs.attr("d", arcGen);

    arcs
        .enter()
        .append('path')
        .attr('fill', function (d) { return (window.pieControls.color(d.data.key)); })
        .attr("stroke", "black")
        .style("stroke-width", "2px")
        .style("opacity", 1)
        .merge(arcs)
        .attr('d', arcGen)
        .each(function (d) { this._current = d; });

    var text = arc.arc
        .selectAll('text')
        .data([data['a']]);

    text.enter()
        .append('text')
        .attr("text-anchor", "middle")
        .style("pointer-events", "none")
        .style("user-select", "none")
        .merge(text)
        .text(function (d) { return (d * 100).toFixed(0) });
};


window.pieControls.init = function (netInstance, ref, radius) {
    var margin = 10;
    var width = radius * 2 + 2 * margin;
    var height = radius + radius * Math.cos(Math.PI * (1 - window.pieControls.angleC)) + 2 * margin;
    const id = '#' + ref;
    var svg = d3.select(id)
        .append("svg")
        .on('mousemove', function (d, i) {
            if (d3.event.buttons % 2 === 1) {
                var node = d3.select(this).node();
                var bbox = node.getBBox();
                var mouse = d3.mouse(node);
                var unbounded = 1 - (mouse[1] / (bbox.height + bbox.y * 2));
                var value = Math.min(1, Math.max(0, unbounded));
                window.pieControls.updateValue(ref, value);
                netInstance.invokeMethodAsync('UpdateValue', value);
            }
        })
        .attr("viewBox", "0 0 160 130")
        .append("g")
        .attr("transform", "translate(" + width / 2 + "," + (radius + margin) + ")");
    window.pieControls.arcs[id] = { arc: svg, radius: radius, netInstance: netInstance };
    window.pieControls.updateValue(ref, 0.3);
};