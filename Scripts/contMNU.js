// ============================================================================
// contMNU.js — Контекстное меню и кнопка «Наверх»
// ============================================================================
// 1. Контекстное меню — показывается по правому клику на элементах с классом
//    .contextmenu-trigger (вместо перехвата всех правых кликов на странице).
// 2. Кнопка «Наверх» — появляется при прокрутке вниз, плавно возвращает наверх.
// ============================================================================

// === Контекстное меню ===

var menu = document.querySelector('.menu');

/**
 * Показать контекстное меню в указанных координатах.
 * @param {number} x - Координата X (пиксели от левого края)
 * @param {number} y - Координата Y (пиксели от верхнего края)
 */
function showMenu(x, y) {
    if (!menu) return;
    menu.style.left = x + 'px';
    menu.style.top = y + 'px';
    menu.classList.add('show-menu');
}

/**
 * Скрыть контекстное меню.
 */
function hideMenu() {
    if (!menu) return;
    menu.classList.remove('show-menu');
}

/**
 * Обработчик правого клика — показать контекстное меню.
 * ИСПРАВЛЕНИЕ: теперь перехватывает только клики на элементах
 * с классом .contextmenu-trigger, не блокируя стандартное меню браузера.
 */
function onContextMenu(e) {
    e.preventDefault();
    showMenu(e.pageX, e.pageY);
    document.addEventListener('mousedown', onMouseDown, false);
}

/**
 * Обработчик любого клика мыши — скрыть контекстное меню.
 */
function onMouseDown(e) {
    hideMenu();
    document.removeEventListener('mousedown', onMouseDown);
}

// Привязка контекстного меню только к элементам с классом .contextmenu-trigger
// (не перехватываем правый клик по всей странице)
var triggers = document.querySelectorAll('.contextmenu-trigger');
for (var i = 0; i < triggers.length; i++) {
    triggers[i].addEventListener('contextmenu', onContextMenu, false);
}

// === Кнопка «Наверх» ===

(function ($) {
    /**
     * Инициализация кнопки «Наверх».
     * Кнопка появляется при прокрутке более 50px от верха.
     * По клику — плавная прокрутка наверх за 600мс.
     */
    function backToTop() {
        var button = $('.back-to-top');
        // Показать/скрыть кнопку при прокрутке
        $(window).on('scroll', function () {
            if ($(this).scrollTop() >= 50) {
                button.fadeIn();
            } else {
                button.fadeOut();
            }
        });
        // Плавная прокрутка наверх при клике
        button.on('click', function (e) {
            e.preventDefault();
            $('html, body').animate({ scrollTop: 0 }, 600);
        });
    }
    backToTop();
})(jQuery);
