Dcl-Proc PrintInfo Export;
  Dcl-Pi PrintInfo;
    Name Char(10) Const;
    Age  Int(5)   Const;
  End-Pi;
  
  Dsply 'You are ' + %Trim(Name) + ', you are ' + %Char(Age) + ' years old';
End-Proc;