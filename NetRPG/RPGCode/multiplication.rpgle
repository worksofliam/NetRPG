
Dcl-S MyInt    Int(5);
Dcl-S MyPacked Packed(9:2);

MyInt = 10 * 2;
MyPacked = 324.33 * 2;

Return %Char(MyInt) + %Char(MyPacked);