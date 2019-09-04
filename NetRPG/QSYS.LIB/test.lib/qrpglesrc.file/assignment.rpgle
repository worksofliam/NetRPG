Dcl-S MyChar   Char(15);
Dcl-S MyInt    Int(5);
Dcl-S MyPacked Packed(9:2);

MyChar   = 'Hello world';
MyInt    = 2342;
MyPacked = 1234.56;

Return MyChar + %Char(MyInt) + %Char(MyPacked);