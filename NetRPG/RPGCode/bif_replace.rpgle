Dcl-S resulta Char(20);
Dcl-S resultb Char(20);
Dcl-S resultc Char(30);

resulta = %replace('Canada':'Toronto, ON':10);
resultb = %replace('Toronto':'Windsor, ON');
resultc = %replace('Scarborough':'Toronto, Ontario, Canada':1:7);

return %trim(resulta) + %trim(resultb) + %trim(resultc);