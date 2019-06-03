window.keyboard = {};
document.onkeydown = function (e) {
    var instance = window.keyboard['instance'];
    if (instance) {
        e = e || window.event;
        instance.invokeMethodAsync('OnKeyDown', e.key);// use e.keyCode
    }
};

window.keyboard.init = function (netInstance) {
    window.keyboard['instance'] = netInstance;
}