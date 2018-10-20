
Dcl-S MyVar Char(50);
Dcl-S MyInt Int(5);
Dcl-S MyPacked Packed(9:2);
Dcl-S MyZoned Packed(9:2);

MyVar = '5';
MyInt = %Int(MyVar);
Dsply %Char(MyInt + 5);

MyPacked = %Dec('1234.567':9:2);
Dsply %Char(MyPacked);

Return;