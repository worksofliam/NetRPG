
Dcl-Ds MyDS Qualified;
  Dcl-Subf Boom    Char(10);
  Dcl-Subf Boom2   Char(10) Dim(3);
End-Ds;

MyDS.Boom = 'Hello';
MyDS.Boom2(2) = 'World';

Return MyDS.Boom + MyDS.Boom2(2);