**FREE

Dcl-Ds MyDS Qualified Dim(2);
  Dcl-Subf Boom    Char(10);
  Dcl-Subf Boom2   Char(10) Dim(3);
End-Ds;

MyDS(1).Boom = 'Hello';
MyDS(2).Boom2(2) = 'World';

Return MyDS(1).Boom + MyDS(2).Boom2(2);