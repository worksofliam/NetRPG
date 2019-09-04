Dcl-S Parm1 Char(5) Inz('Hello');
Dcl-S Parm2 Char(5) Inz('!');

Return MyProc(Parm1:Parm2);

Dcl-Proc MyProc;
  Dcl-Pi MyProc Char(10);
    Dcl-Parm MyParm  Char(5);
    Dcl-Parm MyOther Char(5);
  End-Pi;

  Return %Trim(MyParm) + %Trim(MyOther);
End-Proc;