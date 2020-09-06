**FREE

Dcl-Ds MyDS Qualified;
  Dcl-Subf Boom    Char(10);
  Dcl-Subf Boom2   Char(10);
  Dcl-Subf SomeNum Int(5);
End-Ds;

MyDS.Boom    = 'Hello';
MyDS.Boom2   = 'World';
MyDS.SomeNum = 10;

Return MyDS.Boom + MyDS.Boom2 + %Char(MyDS.SomeNum);