let dark = 'js/highlight/styles/monokai.min.css';
let light = 'js/highlight/styles/stackoverflow-light.min.css';
window.themeIsDark = function (isDark) {
    if (isDark) {
        document.getElementById("theme").href = dark;
    } else {
        document.getElementById("theme").href = light;
    }
};
window.highlightSnippet = function () {
    document.querySelectorAll('pre code').forEach((el) => {
        hljs.highlightElement(el);
    });
};