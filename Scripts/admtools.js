// ============================================================================
// admtools.js — Инструменты администратора
// ============================================================================
// Функции для управления CNC-программами и листами:
// - Отметка программы как выполненной (donecnc)
// - Удаление программы (delcnc)
// - Отметка листа (marksheet)
// - Редактирование прихода металла (editSheetArrival, sendSheetModification)
// ============================================================================

/**
 * Отметить CNC-программу как выполненную.
 * После успешного ответа скрывает строку из таблицы.
 * Если заказ полностью готов — показывает уведомление.
 * @param {number} id  - ID программы в базе данных
 * @param {string} url - URL метода DoneCNC на сервере
 */
async function donecnc(id, url) {
    try {
        let response = await fetch(url + id);
        response = await response.json();
        // Если статус > 0 — операция успешна, скрываем строку
        if (response.status > 0) {
            let s = document.getElementById('mymnu_' + id).parentElement.parentElement.classList;
            s.remove(0);
            s.add("hiddenrow_1");
        }
        // Показываем список готовых заказов (если есть)
        for (var i = 0; i < response.Lst.length; i++) {
            alert('Заказ готов: ' + response.Lst[i]);
        }
    }
    catch (e) {
        console.error('Ошибка при отметке программы:', e);
    }
}

/**
 * Удалить CNC-программу из базы данных.
 * @param {number} id  - ID программы
 * @param {string} url - URL метода DelCNC на сервере
 */
async function delcnc(id, url) {
    // Запрос подтверждения перед удалением
    if (!confirm('Удалить программу #' + id + '?')) return;
    try {
        let response = await fetch(url + id);
        response = await response.json();
        // Если ответ > 0 — удаление успешно, скрываем строку
        if (response > 0) {
            let s = document.getElementById('mymnu_' + id).parentElement.parentElement.classList;
            s.remove(0);
            s.add("hiddenrow_1");
        }
    }
    catch (e) {
        console.error('Ошибка при удалении программы:', e);
    }
}

/**
 * Отметить лист как использованный в программе.
 * @param {number} id  - ID листа
 * @param {string} url - URL метода на сервере
 */
async function marksheet(id, url) {
    try {
        let response = await fetch(url + id);
        response = await response.json();
        if (response > 0) {
            let s = document.getElementById('mymnu_' + id).parentElement.parentElement.classList;
            s.remove(0);
            s.add("hiddenrow_1");
        }
    }
    catch (e) {
        console.error('Ошибка при отметке листа:', e);
    }
}

// ============================================================================
// Редактирование прихода металла (модальное окно на странице SheetsArrival)
// ============================================================================

// Хранение старых и новых значений для редактирования прихода
var _DateOld, _DocumentOld, _ticknessOld, _MatherialOld, _WidthOld, _heigthOld, _QuantityOld, _ownerOld;
var _DateNew, _documentNew, _ticknessNew, _matherialNew, _widthNew, _heigthNew, _qtyNew, _ownerNew;
var _sheetFree; // Количество свободных (не привязанных к программам) листов

/**
 * Найти индекс элемента в списке <select> по значению.
 * @param {HTMLSelectElement} sel - Элемент select
 * @param {string} val - Искомое значение
 * @returns {number} Индекс или -1 если не найден
 */
function FIndex(sel, val) {
    for (var i = 0; i < sel.length; i++) {
        if (sel[i].value == val) return i;
    }
    return -1;
}

/**
 * Открыть модальное окно редактирования прихода листов.
 * Заполняет форму текущими данными позиции и запрашивает у сервера
 * количество свободных листов (которые можно удалить).
 *
 * @param {string} url        - URL метода EditSheetArrival
 * @param {string} urlgetQtyMin - URL метода EditSheetArrivalMinQty
 * @param {string} _Date      - Дата прихода
 * @param {string} _document  - Номер документа
 * @param {number} _tickness  - Толщина
 * @param {number} _matherial - Код материала
 * @param {number} _width     - Ширина листа
 * @param {number} _heigth    - Высота листа
 * @param {number} _qty       - Количество листов
 * @param {string} _owner     - Владелец/заказчик
 */
async function editSheetArrival(url, urlgetQtyMin, _Date, _document, _tickness, _matherial, _width, _heigth, _qty, _owner) {
    // Заполнить форму текущими значениями
    document.getElementById('Date').value = _Date;
    document.getElementById('DOC').value = _document;
    document.getElementById('OWNER').value = _owner;
    document.getElementById('QUANTITY').value = _qty;
    document.getElementById('matsel').selectedIndex = _matherial;

    var s = document.getElementById('Selecttikn');
    s.selectedIndex = FIndex(s.options, _tickness);

    document.getElementById("SheetWidth").value = _width;
    document.getElementById("SheetHeigth").value = _heigth;

    // Определить стандартный или нестандартный размер
    s = document.getElementById('sizesel');
    if (FIndex(s.options, 's' + _width + 'x' + _heigth) >= 0) {
        s.selectedIndex = FIndex(s.options, 's' + _width + 'x' + _heigth);
        hidess();
    } else {
        s.selectedIndex = FIndex(s.options, 'OTHERSIZE');
        showss();
    }

    // Запросить у сервера количество свободных (не начатых) листов
    var qwestion = urlgetQtyMin + '?' + new URLSearchParams({
        DateOld: _Date,
        DocumentOld: _document,
        MatherialOld: _matherial,
        ticknessOld: _tickness,
        ownerOld: _owner,
        WidthOld: _width,
        heigthOld: _heigth,
        QuantityOld: _qty
    });

    try {
        let response = await fetch(qwestion);
        response = await response.json();
        _sheetFree = 0 + response.quantity;
    }
    catch (e) {
        console.error('Ошибка получения данных о листах:', e);
    }

    // Минимальное количество = текущее минус свободные (нельзя убрать уже начатые)
    document.getElementById('QUANTITY').min = _qty - _sheetFree;
    updatemass();

    // Показать модальное окно
    document.getElementById('myModal').style.display = 'block';

    // Если часть листов уже привязана к программам — заблокировать изменение параметров
    if (_sheetFree !== _qty) {
        document.getElementById('matsel').disabled = true;
        document.getElementById('Selecttikn').disabled = true;
        document.getElementById("SheetWidth").disabled = true;
        document.getElementById("SheetHeigth").disabled = true;
    } else {
        document.getElementById('matsel').disabled = false;
        document.getElementById('Selecttikn').disabled = false;
        document.getElementById("SheetWidth").disabled = false;
        document.getElementById("SheetHeigth").disabled = false;
    }

    // Сохранить старые значения для отправки на сервер
    _DateOld = _Date;
    _DocumentOld = _document;
    _MatherialOld = _matherial;
    _ticknessOld = _tickness;
    _ownerOld = _owner;
    _WidthOld = _width;
    _heigthOld = _heigth;
    _QuantityOld = _qty;
}

/**
 * Отправить изменения прихода листов на сервер.
 * Сравнивает старые и новые значения, сервер выполняет:
 * - Удаление лишних листов (если количество уменьшилось)
 * - Добавление новых листов (если увеличилось)
 * - Обновление параметров оставшихся листов
 *
 * @param {string} url - URL метода EditSheetArrival
 */
async function sendSheetModification(url) {
    // Собрать новые значения из формы
    _DateNew = document.getElementById('Date').value;
    _documentNew = document.getElementById('DOC').value;
    _ownerNew = document.getElementById('OWNER').value;
    _qtyNew = document.getElementById('QUANTITY').value;
    _matherialNew = document.getElementById('matsel').options[document.getElementById('matsel').selectedIndex].value;
    _ticknessNew = document.getElementById('Selecttikn').options[document.getElementById('Selecttikn').options.selectedIndex].value;
    _widthNew = document.getElementById("SheetWidth").value;
    _heigthNew = document.getElementById("SheetHeigth").value;

    // Сформировать запрос со старыми и новыми параметрами
    var qwestion = url + '?' + new URLSearchParams({
        DateOld: _DateOld,
        DocumentOld: _DocumentOld,
        MatherialOld: _MatherialOld,
        ticknessOld: _ticknessOld,
        ownerOld: _ownerOld,
        WidthOld: _WidthOld,
        heigthOld: _heigthOld,
        QuantityOld: _QuantityOld,
        DateNew: _DateNew,
        DocumentNew: _documentNew,
        MatherialNew: _matherialNew,
        ticknessNew: _ticknessNew,
        ownerNew: _ownerNew,
        WidthNew: _widthNew,
        heigthNew: _heigthNew,
        QuantityNew: _qtyNew
    });

    try {
        let response = await fetch(qwestion);
        response = await response.json();
        if (response.operation) {
            alert(
                'Удалено    : ' + response.deleted + ' листов\n' +
                'Добавлено  : ' + response.inserted + ' листов\n' +
                'Обработано : ' + response.changed + ' листов'
            );
            // Закрыть модальное окно и обновить страницу
            document.getElementById('myModal').style.display = "none";
            location.reload();
        }
    }
    catch (e) {
        console.error('Ошибка при сохранении изменений:', e);
        alert('Ошибка при сохранении: ' + e.message);
    }
}
