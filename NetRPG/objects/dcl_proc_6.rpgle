**FREE

Dcl-S Result Char(6);
Dcl-S TestVar Char(1);

Result = MyProc();
Result = %Trim(Result) + MyProc(TestVar);
Result = %Trim(Result) + MyProc(TestVar:TestVar);

Return Result;

Dcl-Proc MyProc;
  Dcl-Pi MyProc Char(2);
    Dcl-Parm Parm1 Char(10);
    Dcl-Parm Parm2 Char(10);
  End-Pi;

  Dcl-S Ind1 Ind;
  Dcl-S Ind2 Ind;

  Ind1 = *Off;
  Ind2 = *Off;

  If (Parm1 <> *NULL);
    Ind1 = *On;
  Endif;

  If Parm2 <> *NULL;
    Ind2 = *On;
  Endif;

  Return Ind1 + Ind2;
End-Proc;