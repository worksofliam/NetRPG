**FREE

Dcl-S Output Char(15);
Dcl-S hello Char(10) Inz('hello');


Dsply '';
NoChangeParm('world');
NoChangeParm(hello);
Output = hello;




Return hello;

Dcl-Proc NoChangeParm;
  Dcl-Pi NoChangeParm Char(10);
    Dcl-Parm ValueParm Char(10) Value;
  End-Pi;
  
  Dsply 'Before: ' + ValueParm;
  ValueParm = %TrimR(ValueParm) + '!';
  Dsply 'After: ' + ValueParm;
End-Proc;