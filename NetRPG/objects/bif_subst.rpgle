**FREE

Dcl-S resulta Char(20);
Dcl-S resultb Char(20);
Dcl-S resultc Char(20);
Dcl-S resultd Char(20);

resulta = %subst('Hello World':1:5);

resultb = %subst('Hello World':7);
resultc = %subst('Hello World':7:3);
resultd = %subst('abcd' + 'efgh':4:3);

return %trim(resulta) + %trim(resultb) + %trim(resultc) + %trim(resultd);