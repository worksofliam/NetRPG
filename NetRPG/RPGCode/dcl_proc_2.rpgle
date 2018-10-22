Dcl-S MyGlobal Char(5);

//MyGlobal = 'Hello';

MyGlobal = MyProc();

Return MyGlobal;

Dcl-Proc MyProc;
  Dcl-Pi Proc Char(5) End-Pi;
  Return 'World';
End-Proc;