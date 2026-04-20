// ============================================================================
// ShowHide.js — Управление видимостью строк в таблицах
// ============================================================================
// Показывает/скрывает строки таблицы в зависимости от состояния чекбоксов.
// Строки разделены на два класса:
//   hiddenrow_0/1     — завершённые (скрыты по умолчанию, чекбокс cb1)
//   alwaysshowrow_0/1 — незавершённые (видны по умолчанию, чекбокс cb2)
// Суффикс _0/_1 используется для чередования цвета строк.
// ============================================================================

/**
 * Показать или скрыть группу строк по ID чекбокса.
 * @param {string} cbid - ID чекбокса ("cb1" или "cb2")
 */
function showOrHide(cbid) {
    var cb = document.getElementById(cbid);
    if (!cb) return;

    if (cbid == "cb1") {
        // Чекбокс «Отобразить готовые» — управляет скрытыми строками
        toggleRows("hiddenrow_0", cb.checked);
        toggleRows("hiddenrow_1", cb.checked);
    }
    else if (cbid == "cb2") {
        // Чекбокс «Отобразить неготовые» — управляет видимыми строками
        toggleRows("alwaysshowrow_0", cb.checked);
        toggleRows("alwaysshowrow_1", cb.checked);
    }
}

/**
 * Переключить видимость строк по CSS-классу.
 * @param {string} className - CSS-класс строк
 * @param {boolean} show - Показать (true) или скрыть (false)
 */
function toggleRows(className, show) {
    var elems = document.getElementsByClassName(className);
    for (var i = 0; i < elems.length; i++) {
        elems[i].style.display = show ? "table-row" : "none";
    }
}

/**
 * Показать/скрыть подменю (развернуть детали программы).
 * При нажатии на имя программы показывается блок с деталями и canvas.
 * Все остальные блоки скрываются.
 * @param {string} elId - ID элемента div с подменю
 */
function showSubMNU(elId) {
    var elems = document.getElementsByClassName("vis");
    for (var i = 0; i < elems.length; i++) {
        if (elems[i].id == elId) {
            // Переключить видимость: если уже открыт — скрыть, иначе показать
            elems[i].style.display = (elems[i].style.display == "block") ? "none" : "block";
        } else {
            // Скрыть все остальные подменю
            elems[i].style.display = "none";
        }
    }
}
