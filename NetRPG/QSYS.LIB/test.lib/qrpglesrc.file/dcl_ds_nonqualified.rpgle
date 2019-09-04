Dcl-Ds MyDS;
  Dcl-Subf Hello1 Char(5);
  Dcl-Subf Hello2 Packed(5:2);
End-Ds;

Hello1 = 'Hello';
Hello2 = 123.45;

Return Hello1 + %Char(Hello2);