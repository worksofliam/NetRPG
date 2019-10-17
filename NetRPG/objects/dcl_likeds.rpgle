**FREE

Dcl-Ds MyDS Qualified;
  Dcl-Subf Field1 Char(5);
End-Ds;

Dcl-Ds OtherDS LikeDS(MyDS);
End-Ds;

OtherDS.Field1 = 'Hello';

Return MyDS.Field1 + OtherDS.Field1;