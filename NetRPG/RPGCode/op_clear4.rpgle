Dcl-Ds MyStruct Qualified;
  Dcl-subf field1 Char(11);
  Dcl-subf field2 Int(5);
End-Ds;

MyStruct.field2 = 12;

Reset MyStruct.field2;

Return MyStruct.field2;