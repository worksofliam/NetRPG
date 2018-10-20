
Dcl-S MyVar Char(50);
Dcl-S MyInt Int(5);
Dcl-S MyPacked Packed(9:2);
Dcl-S MyZoned Packed(9:2);

Dcl-S MyArray Int(10) Dim(10);

Dcl-Ds MyDS Qualified;
  Dcl-Subf Boom Char(5);
End-Ds;

Dcl-Ds MyDSArr Qualified Dim(5);
  Dcl-Subf Boom Char(5);
End-Ds;

MyDS.Boom = 'Hello';
Dsply MyDS.Boom;

MyDSArr(1).Boom = 'Hello';
Dsply MyDSArr(1).Boom;

MyArray(1) = 1;
MyArray(2) = 2;
MyArray(3) = 3;
MyArray(4) = 4;
MyArray(5) = 5;

MyInt = %Lookup(0:MyArray);
Dsply %Char(MyInt);

Return;