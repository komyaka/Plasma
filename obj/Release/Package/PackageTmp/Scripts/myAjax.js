var xmlRequest;
try {
    // Этот код работает, если XMLHttpRequest является частью JavaScript
    xmlRequest = new XMLHttpRequest();
}
catch (err) {
    // В противном случае необходим объект ActiveX
    xmlRequest = new ActiveXObject("Microsoft.XMLHTTP");
}
function checkupdaterequest() {
    if (xmlRequest.readyState == 4) {
        if (xmlRequest.status == 200) {
            var response = JSON.parse(xmlRequest.responseText);
            if (+response.substr(6, 13) > Date.parse(document.getElementById('Currenttime').innerHTML)) {
                location.reload(true);
                //document.getElementById('PleaceUpdate').style.display = 'block';
                //clearInterval(timerId);
            }
        }
    }
}
function MyRequest(Url,fun)
{
    xmlRequest.open("POST", Url);
    xmlRequest.onreadystatechange = fun;
    xmlRequest.send(null);
}