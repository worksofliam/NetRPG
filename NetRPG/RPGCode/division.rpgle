
Dcl-S MyInt    Int(5);
Dcl-S MyPacked Packed(9:2);

MyInt = 20 /5;
MyPacked = 20.5 / 5;

Return %Char(MyInt) + %Char(MyPacked);