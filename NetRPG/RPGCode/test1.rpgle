
Dcl-S MyVar Char(50);
Dcl-S MyInt Int(5);
Dcl-S MyPacked Packed(9:2);
Dcl-S MyZoned Packed(9:2);

MyPacked = 3123.24 * 2;
Dsply %EditC(MyPacked:'1':'$');

MyZoned = 234.25 * 2;
Dsply %EditC(MyZoned:'B':'$');

Return;