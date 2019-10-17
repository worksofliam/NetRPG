**FREE

Dcl-S MyChar   Char(5)     Inz('Hello');
Dcl-S MyInt    Int(5)      Inz(42);
Dcl-S MyPacked Packed(9:2) Inz(1234.56);

Dcl-Ds MyDS Qualified;
  Dcl-Subf Field1 Char(5) Inz('World');
End-Ds;

Dcl-Ds MyDSArray Qualified Dim(2);
  Dcl-Subf Field1 Int(5) Inz(55);
End-Ds;

Return MyChar + MyDS.Field1 + %Char(MyInt) + %Char(MyPacked) + %Char(MyDSArray(1).Field1);