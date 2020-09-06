**FREE

Dcl-Ds TestDs Qualified;
  Dcl-Subf Boom Char(5);
End-Ds;

MyProc(TestDs);

Return TestDs.Boom;

Dcl-Proc MyProc;
  Dcl-Pi MyProc;
    Dcl-Parm TheDS LikeDS(TestDs);
  End-Pi;

  TheDS.Boom = 'Hello';
End-Proc;