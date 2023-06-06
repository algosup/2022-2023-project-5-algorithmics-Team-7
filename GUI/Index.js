element1 =    document.getElementById('0.1'),
element2 =    document.getElementById('1.4')

element3 =    document.getElementById('0.2'),
element4 =    document.getElementById('1.2')

element5 =    document.getElementById('0.4'),
element6 =    document.getElementById('1.1')


element11 =    document.getElementById('2.1'),
element21 =    document.getElementById('3.3')

element31 =    document.getElementById('2.2'),
element41 =    document.getElementById('3.0')

element51 =    document.getElementById('2.4'),
element61 =    document.getElementById('3.2')

new LeaderLine(element1, element2, {color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});
new LeaderLine(element3, element4, {color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});
new LeaderLine(element5, element6, { color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});


new LeaderLine(element11, element21, {color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});
new LeaderLine(element31, element41, {color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});
new LeaderLine(element51, element61, { color: 'red', startSocket: 'bottom', endSocket: 'top', path : 'straight'});

