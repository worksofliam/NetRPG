Dcl-S MyGlobal Char(5);

MyGlobal = 'Hello';

MyProc();

Return MyGlobal;

Dcl-Proc MyProc;
  MyGlobal = 'World';
End-Proc;