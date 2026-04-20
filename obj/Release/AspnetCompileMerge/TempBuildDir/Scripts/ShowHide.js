function showOrHide(cbid) {
    cb = document.getElementById(cbid);
    if (cbid == "cb1") {
        var elems = document.getElementsByClassName("hiddenrow_0");
        for (var i = 0; i < elems.length; i++) {
            if (cb.checked) elems[i].style.display = "table-row";
            else elems[i].style.display = "none";
        }
        var elems = document.getElementsByClassName("hiddenrow_1");
        for (var i = 0; i < elems.length; i++) {
            if (cb.checked) elems[i].style.display = "table-row";
            else elems[i].style.display = "none";
        }
    }
    else if (cbid == "cb2") {
        var elems = document.getElementsByClassName("alwaysshowrow_0");
        for (var i = 0; i < elems.length; i++) {
            if (cb.checked) elems[i].style.display = "table-row";
            else elems[i].style.display = "none";
        }
        var elems = document.getElementsByClassName("alwaysshowrow_1");
        for (var i = 0; i < elems.length; i++) {
            if (cb.checked) elems[i].style.display = "table-row";
            else elems[i].style.display = "none";
        }
    }
}
function showSubMNU(elId) {
    var elems = document.getElementsByClassName("vis");
    for (var i = 0; i < elems.length; i++) {
        if (elems[i].id == elId) { elems[i].style.display = "block"; } else { elems[i].style.display = "none"; }
    }
}
