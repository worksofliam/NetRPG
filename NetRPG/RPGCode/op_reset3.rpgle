**FREE

Dcl-Ds MyStruct Qualified;
  Dcl-subf field1 Char(11) Inz('Hello world');
  Dcl-subf field2 Int(5)   Inz(134);
End-Ds;

MyStruct.field1 = 'Hello friend';
MyStruct.field2 = 12;

Reset MyStruct;

Return MyStruct.field1 + %char(MyStruct.field2);