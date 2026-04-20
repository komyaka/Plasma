async function donecnc(id,url) {
    try {
        let response = await fetch(url + id);
        response = await response.json();
        if (response.status > 0) { let s = document.getElementById('mymnu_' + id).parentElement.parentElement.classList; s.remove(0); s.add("hiddenrow_1"); }
        for (var i = 0; i < response.Lst.length;i++)alert('заказ готов' + response.Lst[i]);
    }
    catch (e) {
        console.log(e);
    }
}
async function delcnc(id, url) {
    try {
        let response = await fetch(url + id);
        response = await response.json();
        if (response > 0) { let s = document.getElementById('mymnu_' + id).parentElement.parentElement.classList; s.remove(0); s.add("hiddenrow_1"); }
    }
    catch (e) {
        console.log(e);
    }
}
async function markheet(id, url) {
    try {
        let response = await fetch(url + id);
        response = await response.json();
        if (response > 0) { let s = document.getElementById('mymnu_' + id).parentElement.parentElement.classList; s.remove(0); s.add("hiddenrow_1"); }
    }
    catch (e) {
        console.log(e);
    }
}
// данные для управления записью о приходе металла
var _DateOld, _documentOld, _ticknessOld, _matherialOld, _widthOld, _heigthOld, _qtyOld, _ownerOld;
var _DateNew, _documentNew, _ticknessNew, _matherialNew, _widthNew, _heigthNew, _qtyNew, _ownerNew;
var _sheetFree;
function FIndex(sel, val) {
    for (i = 0; i < sel.length; i++) { if (sel[i].value == val) return i; } return -1;
}
async function editSheetArrival(url, urlgetQtyMin, _Date, _document, _tickness, _matherial, _width, _heigth, _qty, _owner) {
    // Заполнить форму С параматрами листа
        document.getElementById('Date').value = _Date;
        document.getElementById('DOC').value = _document;
        document.getElementById('OWNER').value = _owner;
        document.getElementById('QUANTITY').value = _qty;
        document.getElementById('matsel').selectedIndex = _matherial;
        var s = document.getElementById('Selecttikn');
        s.selectedIndex = FIndex(s.options, _tickness);
        document.getElementById("SheetWidth").value = _width;
        document.getElementById("SheetHeigth").value = _heigth;
        s = document.getElementById('sizesel');
        if (FIndex(s.options, 's' + _width + 'x' + _heigth) >= 0) { s.selectedIndex = FIndex(s.options, 's' + _width + 'x' + _heigth); hidess();}
        else { s.selectedIndex = FIndex(s.options, 'OTHERSIZE'); showss();}
// получить от сервера количество не начатых листов с конкретной позиции.
        qwestion = urlgetQtyMin + '?' + new URLSearchParams({
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
        console.log(e);
    }
    //нельзя убрать листы из прихода котрые уже в работе.
    document.getElementById('QUANTITY').min = _qty-_sheetFree;
    updatemass();
    //показать модальное окно,
    document.getElementById('myModal').style.display = 'block';
    // если часть листов уже отмечена программами то нельзя изменить: материал и размер листа
    // поэтому соответствующие поля на форме отключены
    if (_sheetFree !== _qty)
    {
        document.getElementById('matsel').disabled = true;
        document.getElementById('Selecttikn').disabled = true;
        document.getElementById("SheetWidth").disabled = true;
        document.getElementById("SheetHeigth").disabled = true;
    }
    _DateOld= _Date;
    _DocumentOld= _document;
    _MatherialOld= _matherial;
    _ticknessOld= _tickness;
    _ownerOld= _owner;
    _WidthOld= _width;
    _heigthOld= _heigth;
    _QuantityOld = _qty;
}

// отправить на сервер изменения по приходу позиции
async function sendSheetModification(url)
{
    _DateNew = document.getElementById('Date').value;
    _documentNew = document.getElementById('DOC').value;
    _ownerNew = document.getElementById('OWNER').value;
    _qtyNew = document.getElementById('QUANTITY').value;
    _matherialNew = document.getElementById('matsel').options[document.getElementById('matsel').selectedIndex].value;
    _ticknessNew = document.getElementById('Selecttikn').options[document.getElementById('Selecttikn').options.selectedIndex].value;
    _widthNew = document.getElementById("SheetWidth").value;
    _heigthNew = document.getElementById("SheetHeigth").value;

  //  try {
        qwestion = url + '?' + new URLSearchParams({
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
    let response = await fetch(qwestion);     
 /*   }
    catch (e) {
        console.log(e);
    }*/
    response = await  response.json();
    if (response.operation) {
        alert(
            'Удалено    :' + response.deleted + 'листов \n' +
            'Добавлено  :' + response.inserted + 'листов \n' +
            'Обработано :' + response.changed + 'листов \n' 
            );
        modal.style.display = "none";
    }

}
