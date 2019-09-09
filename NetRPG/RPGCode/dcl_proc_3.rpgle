**FREE

Dcl-S MyGlobal Char(5);

MyGlobal = 'Hello';

MyProc(MyGlobal);

Return MyGlobal;

Dcl-Proc MyProc;
  Dcl-Pi MyProc;
    Dcl-Parm MyParm Char(5);
  End-Pi;

  MyParm = 'World';
End-Proc;