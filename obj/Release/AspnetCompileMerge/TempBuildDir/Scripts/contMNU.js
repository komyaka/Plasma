var menu = document.querySelector('.menu');
function showMenu(x, y) {
    menu.style.left = x + 'px';
    menu.style.top = y + 'px';
    menu.classList.add('show-menu');
}
function hideMenu() {
    menu.classList.remove('show-menu');
}
function onContextMenu(e) {
    e.preventDefault();
    showMenu(e.pageX, e.pageY);
    document.addEventListener('mousedown', onMouseDown, false);
}
function onMouseDown(e) {
    hideMenu();
    document.removeEventListener('mousedown', onMouseDown);
}
document.addEventListener('contextmenu', onContextMenu, false);
 (function($) {

function backToTop() {
    let button = $('.back-to-top');
    $(window).on('scroll', () => {
        if ($(this).scrollTop() >= 50) {
            button.fadeIn();
        } else {
            button.fadeOut();
        }
    });
    button.on('click', (e) => {
        e.preventDefault();
        $('html').animate({ scrollTop: 0 }, 1000);
    })
}
backToTop();
})(jQuery);