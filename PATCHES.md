# Патчи — конкретные исправления в коде

Применяйте эти изменения к исходным файлам проекта.

---

## 1. _LayoutMenu.cshtml — Лишняя закрывающая скобка

**Файл:** `Views/Shared/_LayoutMenu.cshtml`

**Было (строки 14-17):**
```cshtml
@if (((ViewBag.User.Privilegies & 2) != 0))
{
    <li>@Html.ActionLink("Программы", "Index", "Home")</li>
}
}
```

**Стало (убрана лишняя `}`):**
```cshtml
@if (((ViewBag.User.Privilegies & 2) != 0))
{
    <li>@Html.ActionLink("Программы", "Index", "Home")</li>
}
```

> **ВАЖНО:** Полностью исправленный файл находится в `Views/Shared/_LayoutMenu.cshtml` в папке выходных файлов. Также исправлен Bootstrap dropdown (был синтаксис v4, исправлен на v3).

---

## 2. HomeController.cs — FindIndex в CNCQR()

**Файл:** `Controllers/HomeController.cs`, метод `CNCQR()`

**Было:**
```csharp
int Index = partsClass.CNCs.FindIndex(x => x.Id == ID);
if (Index > 0) { ViewBag.CNC = partsClass.CNCs[Index]; ViewBag.found = true; }
```

**Стало (>= 0, т.к. FindIndex возвращает 0 для первого элемента):**
```csharp
int Index = partsClass.CNCs.FindIndex(x => x.Id == ID);
if (Index >= 0) { ViewBag.CNC = partsClass.CNCs[Index]; ViewBag.found = true; }
```

---

## 3. adrController.cs — SQL-опечатка в DeleteSheet()

**Файл:** `Controllers/adrController.cs`, метод `DeleteSheet()`

**Было:**
```csharp
if (id > 0) parts.FreeRequestToBD("Delete * froms heets Where id=" + id.ToString());
```

**Стало:**
```csharp
if (id > 0) parts.FreeRequestToBD("Delete from sheets Where id=" + id.ToString());
```

Также в этом же методе — пропущены пробелы перед `AND`:

**Было:**
```csharp
"and HEIGTH=" + heigth.ToString()+ " and MATHERIAL=" + matherial+(doc=="*"?"":(  "and document="+doc))
```

**Стало (добавлены пробелы):**
```csharp
" and HEIGTH=" + heigth.ToString() + " and MATHERIAL=" + matherial + (doc == "*" ? "" : (" and DOKUMENT='" + doc + "'"))
```

---

## 4. cnc.cs — Ошибка скорости для толщины 1.5

**Файл:** `Models/NC/cnc.cs`, метод `GetRecomendetSpeed()`

**Было:**
```csharp
if (tickn == "1_5") return new _speed(_Process.PL50, 8, 32000, 12000, 240);
```

**Стало (32000 → 3200):**
```csharp
if (tickn == "1_5") return new _speed(_Process.PL50, 8, 3200, 12000, 240);
```

---

## 5. cnc.cs — Дубль M21 вместо M22

**Файл:** `Models/NC/cnc.cs`, метод `addline()`

**Было (обе строки проверяют M21):**
```csharp
if (m.IndexOf("M21") >= 0) { M21count++; cute = true; }
if (m.IndexOf("M21") >= 0) { cute = false; }
```

**Стало (вторая строка — M22, конец резки):**
```csharp
if (m.IndexOf("M21") >= 0) { M21count++; cute = true; }
if (m.IndexOf("M22") >= 0) { cute = false; }
```

---

## 6. NestInfo.cs — Тот же дубль M21/M22

**Файл:** `Models/NC/NestInfo.cs`, метод `addline()`

**Было:**
```csharp
if (m.IndexOf("M21") >= 0) { this.M21Count++; cute = true; }
if (m.IndexOf("M21") >= 0) { cute = false; }
```

**Стало:**
```csharp
if (m.IndexOf("M21") >= 0) { this.M21Count++; cute = true; }
if (m.IndexOf("M22") >= 0) { cute = false; }
```

---

## 7. Site.css — Опечатки

**Файл:** `Content/Site.css`

**Исправление 1 — убрать «висящий» текст `fav`:**
```css
/* Было: */
fav
/* Стало: удалить строку целиком */
```

**Исправление 2 — fill-opacity:**
```css
/* Было: */
fill-opasity: 0.2;

/* Стало: */
fill-opacity: 0.2;
```

**Исправление 3 — fill-rule:**
```css
/* Было: */
fill-rule: none-zero;

/* Стало: */
fill-rule: nonzero;
```

> **ВАЖНО:** Полностью переработанный `Site.css` с новым дизайном находится в папке выходных файлов.

---

## 8. MarkSheet.cshtml — Кириллическая буква в JS-функции

**Файл:** `Views/Home/MarkSheet.cshtml`

**Было (буква «с» — кириллическая U+0441):**
```javascript
Seleсtfield()
{
    // ...
}
```

**Стало (латинская «c» U+0063):**
```javascript
function SelectField()
{
    // Здесь разместить код: если выбран какой-либо ID,
    // то нужно скрыть все остальные options с этим id
}
```

Также в этом же файле в теге `<select>`:
```html
<!-- Было: -->
onchange="Seleсtfield"

<!-- Стало: -->
onchange="SelectField()"
```

---

## 9. SheetsArrival.cshtml — Пропущен аргумент lq

**Файл:** `Views/Home/SheetsArrival.cshtml`

В блоке `else` (когда `x.Name != ""`) в вызове `editSheetArrival` пропущен второй аргумент `lq`:

**Было:**
```html
onclick='editSheetArrival(le,"@x.Date...",...)'
```

**Стало (добавлен lq):**
```html
onclick='editSheetArrival(le,lq,"@x.Date...",...)'
```

---

## 10. Parts.cshtml — Синтаксическая ошибка в HTML

**Файл:** `Views/Home/Parts.cshtml`

**Было (незакрытый тег th):**
```html
<th class="sorted" onclick="...">№</thclass="sorted">
```

**Стало:**
```html
<th class="sorted" onclick="...">№</th>
```

---

## 11. HomeController.cs — SheetsArrival SQL-инъекция (пробелы перед AND)

**Файл:** `Controllers/HomeController.cs`, метод `SheetsArrival()`

**Было (пропущены пробелы, SQL сливается):**
```csharp
if (datestart.Length > 0) qwestion += "AND DATE>" + datestart;
if (dateend.Length > 0) qwestion += "AND DATE<" + dateend;
if (Document.Length > 0) qwestion += "AND DOKUMENT like '%" + Document + "%'";
```

**Стало (добавлены пробелы перед AND):**
```csharp
if (datestart.Length > 0) qwestion += " AND DATE>'" + datestart + "'";
if (dateend.Length > 0) qwestion += " AND DATE<'" + dateend + "'";
if (Document.Length > 0) qwestion += " AND DOKUMENT like '%" + Document + "%'";
```

> **ПРИМЕЧАНИЕ:** В идеале все эти параметры должны передаваться через `SqlParameter` для защиты от SQL-инъекций. Это указано в WORKPLAN.md как приоритетная задача модернизации.

---

## 12. FinCalc.cs — Захардкоженный путь

**Файл:** `Models/FinCalc.cs`

**Было:**
```csharp
string grabFile = @"D:\PlazmaProgs\tmp-" + ...
```

**Стало (использует AppConfig):**
```csharp
string grabFile = AppConfig.TempFilesPath + @"tmp-" + ...
```

**Также в `finepath()`:**

**Было:**
```csharp
string newpath = Constants._plasmaPath;
```

**Стало (уже работает через Constants → AppConfig, но для ясности):**
```csharp
string newpath = AppConfig.CalculationsPath;
```

---

## 13. myAjax.js — Комментарии на русском

Файл `Scripts/myAjax.js` — добавлены русские комментарии. Полная версия в выходных файлах.

---

## 14. Загрузка файлов в модальном окне

Вместо формы с `action="~/adr/newCNC2"` и `method="post"` (которая перенаправляет на новую страницу), теперь используется модальное окно с Vue.js + axios.

**Старый код** (был в `Index.cshtml`):
```html
<form action="~/adr/newCNC2" method="post" enctype="multipart/form-data">
    <input name="uploads" type="file" multiple /><input type="submit" />
</form>
```

**Новый код** — кнопка «Загрузить CNC» открывает модальное окно:
```html
<button class="btn-upload" onclick="openUploadModal()">Загрузить CNC</button>
```

Полная реализация модального окна — в `Views/Home/Index.cshtml` в папке выходных файлов.
