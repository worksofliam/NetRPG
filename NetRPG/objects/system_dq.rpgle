**FREE

Dcl-Pr SendDQ ExtPgm('QSNDDTAQ');
  Dcl-Parm Library Char(10);
  Dcl-Parm Object  Char(10);
  Dcl-Parm Length  Packed(5);
  Dcl-Parm Data    Char(20);
End-Pr;

Dcl-Pr PopDQ ExtPgm('QRCVDTAQ');
  Dcl-Parm Library Char(10);
  Dcl-Parm Object  Char(10);
  Dcl-Parm Length  Packed(5);
  Dcl-Parm Data    Char(20);
End-Pr;

dcl-s lib char(10) inz('MYLIB');
dcl-s obj char(10) inz('MYDQ');
dcl-s len packed(5);
dcl-s data char(20);

data = 'Hello world';
len = %len(data);

SendDQ(lib:obj:len:data);

data = *blank;
len = *zero;

Dsply 'Before: ' + data;
PopDQ(lib:obj:len:data);
Dsply 'After: ' + data;

return data;