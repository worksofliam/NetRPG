**FREE

Return ValueParm(4 + 6);

Dcl-Proc ValueParm;
  Dcl-Pi ValueParm Char(2);
    Dcl-Parm ValueParm Int(10) Value;
  End-Pi;
  
  Return %Char(ValueParm);
End-Proc;