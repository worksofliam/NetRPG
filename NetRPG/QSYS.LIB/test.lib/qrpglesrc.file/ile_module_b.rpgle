Dcl-Proc PrintInfo Export;
  Dcl-Pi PrintInfo Char(52);
    Dcl-Parm Name Char(10) Const;
    Dcl-Parm Age  Int(5)   Const;
  End-Pi;

  Dcl-S LocalField Char(52);

  LocalField = 'You are ' + %Trim(Name) + ', you are ' + %Char(Age) + ' years old';
  
  Dsply '';
  Dsply LocalField;
  Dsply '';

  Return %TrimR(LocalField);
End-Proc;