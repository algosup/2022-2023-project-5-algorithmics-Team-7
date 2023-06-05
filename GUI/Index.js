element1 =    document.getElementById('0.1'),
element2 =    document.getElementById('1.4')

element3 =    document.getElementById('0.2'),
element4 =    document.getElementById('1.2')

element5 =    document.getElementById('0.4'),
element6 =    document.getElementById('1.1')

new LeaderLine(element1, element2, {dash: {animation: true}, color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});
new LeaderLine(element3, element4, {dash: {animation: true}, color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});
new LeaderLine(element5, element6, {dash: {animation: true}, color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});


