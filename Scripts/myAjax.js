// ============================================================================
// myAjax.js — Модуль AJAX-запросов
// ============================================================================
// Обеспечивает асинхронное взаимодействие с сервером:
// - Проверка обновлений данных в таблицах (автообновление страницы)
// - Универсальная функция отправки POST-запросов
// ============================================================================

// Создание объекта XMLHttpRequest для AJAX-запросов
var xmlRequest;
try {
    // Современные браузеры — XMLHttpRequest является частью JavaScript
    xmlRequest = new XMLHttpRequest();
}
catch (err) {
    // Устаревшие версии IE — используется ActiveX-объект
    xmlRequest = new ActiveXObject("Microsoft.XMLHTTP");
}

/**
 * Обработчик ответа на запрос проверки обновлений.
 * Сравнивает дату последнего обновления таблицы на сервере
 * с временной меткой, сохранённой на странице (#Currenttime).
 * Если данные на сервере новее — перезагружает страницу.
 */
function checkupdaterequest() {
    if (xmlRequest.readyState == 4) {
        if (xmlRequest.status == 200) {
            var response = JSON.parse(xmlRequest.responseText);
            // Извлекаем метку времени из JSON-ответа Microsoft (формат /Date(...)/)
            // и сравниваем с меткой времени на странице
            if (+response.substr(6, 13) > Date.parse(document.getElementById('Currenttime').innerHTML)) {
                // Данные обновились на сервере — перезагружаем страницу
                location.reload(true);
            }
        }
    }
}

/**
 * Универсальная функция отправки POST-запроса на сервер.
 * @param {string} Url  - URL-адрес серверного метода
 * @param {function} fun - Функция-обработчик ответа (callback)
 */
function MyRequest(Url, fun) {
    xmlRequest.open("POST", Url);
    xmlRequest.onreadystatechange = fun;
    xmlRequest.send(null);
}
