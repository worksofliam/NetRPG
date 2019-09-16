**FREE

Dcl-S String Char(15);
Dcl-S resulta int(5);
Dcl-S resultb int(5);

String = 'Hello world';

resulta = %Scan('world':String);
resultb = %Scan('World':String);

return %Char(resulta) + %Char(resultb);