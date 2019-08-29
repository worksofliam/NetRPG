Dcl-Proc PrintInfo Export;
  Dcl-Pi PrintInfo;
    Dcl-Parm Name Char(10) Const;
    Dcl-Parm Age  Int(5)   Const;
  End-Pi;
  
  Dsply 'You are ' + %Trim(Name) + ', you are ' + %Char(Age) + ' years old';
End-Proc;