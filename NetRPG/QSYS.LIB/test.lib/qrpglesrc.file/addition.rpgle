Dcl-S MyChar   Char(15);
Dcl-S MyInt    Int(5);
Dcl-S MyPacked Packed(9:2);

MyChar   = 'Hello ' + 'world';
MyInt    = 2042 + 300;
MyPacked = 1034.56 + 200;

Return MyChar + %Char(MyInt) + %Char(MyPacked);