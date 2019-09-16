**FREE

Dcl-S lo Char(26) Inz('abcdefghijklmnopqrstuvwxyz');
Dcl-s hi Char(26) Inz('ABCDEFGHIJKLMNOPQRSTUVWXYZ');
Dcl-s string Char(11);

string = 'hello world';
string = %xlate(lo:hi:string:7);

return string;