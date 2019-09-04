Dcl-Ds MyStruct Qualified;
  Dcl-subf field1 Char(11);
  Dcl-subf field2 Int(5);
End-Ds;

MyStruct.field1 = 'Hello friend';
MyStruct.field2 = 12;

Clear MyStruct;

Return MyStruct.field1 + %char(MyStruct.field2);