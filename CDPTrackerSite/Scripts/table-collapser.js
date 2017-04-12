function toggle_visibility(tbid, lnkid) {
    if (document.getElementsByTagName) {
        var tables = document.getElementsByTagName('table');
        for (var i = 0; i < tables.length; i++) {
            if (tables[i].id == tbid) {
                var trs = tables[i].getElementsByTagName('tr');
                for (var j = 2; j < trs.length; j += 1) {
                    trs[j].bgcolor = '#CCCCCC';
                    if (trs[j].style.display == 'none')
                        trs[j].style.display = '';
                    else
                        trs[j].style.display = 'none';
                }
                break;
            }
        }
    }
    var x = document.getElementById(lnkid);
    if (x.innerHTML == '[+] Expand ')
        x.innerHTML = '[-] Collapse ';
    else
        x.innerHTML = '[+] Expand ';
}
function toggle_visibility_all() {
    if (document.getElementsByTagName) {
        var tableDisplayMode;
        var linkText;
        var toggleAll = document.getElementById('toggle_all');
        if (toggleAll.innerHTML == '[+] Expand All ') {
            tableDisplayMode = '';
            linkText = '[-] Collapse ';
            toggleAll.innerHTML = '[-] Collapse All ';
        } else {
            tableDisplayMode = 'none';
            linkText = '[+] Expand ';
            toggleAll.innerHTML = '[+] Expand All ';
        }

        var tables = document.getElementsByClassName('collapsableTable');
        for (var i = 0; i < tables.length; i++) {
            var trs = tables[i].getElementsByTagName('tr');
            for (var j = 2; j < trs.length; j += 1) {
                trs[j].bgcolor = '#CCCCCC';
                trs[j].style.display = tableDisplayMode;
            }
            var linkId = 'lnk' + i;
            var link = document.getElementById(linkId);
            link.innerHTML = linkText;
        }
    }
}