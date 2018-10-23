
Dcl-Ds MyDS Qualified;
  Dcl-Subf Boom    Char(10);
  Dcl-Subf Boom2   Char(10) Dim(3);
End-Ds;

MyDS.Boom     = 'Hello';
MyDs.Boom2(2) = 'World';

Return MyDS.Boom + MyDs.Boom2(2);