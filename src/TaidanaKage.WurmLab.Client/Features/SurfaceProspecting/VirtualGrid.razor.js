// JavaScript for VirtualGrid component
export function getScrollMetrics(element) {
    return {
        scrollLeft: element.scrollLeft,
        scrollTop: element.scrollTop,
        clientWidth: element.clientWidth,
        clientHeight: element.clientHeight
    };
}
