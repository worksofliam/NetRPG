
Return MyProc();

Dcl-Proc MyProc;
  Dcl-Pi MyProc Char(5) End-Pi;

  Return OtherProc();
End-Proc;

Dcl-Proc OtherProc;
  Dcl-Pi Anything Char(5) End-Pi;

  Return 'World';
End-Proc;