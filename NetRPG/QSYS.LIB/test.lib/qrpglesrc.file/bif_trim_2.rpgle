
Dcl-Ds MyDS Qualified Dim(2);
  Dcl-Subf Boom2   Char(10) Dim(3);
End-Ds;

MyDS(1).Boom2(2) = 'Hello';

Return %Trim(MyDS(1).Boom2(2)) + MyDS(1).Boom2(1);