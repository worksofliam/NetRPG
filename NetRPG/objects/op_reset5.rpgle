**FREE

Dcl-Ds MyDSArr Qualified Dim(2);
  Dcl-Subf Boom Char(10) Inz('Hello');
  Dcl-Subf Num  Int(3)   Inz(21);
End-Ds;

MyDSArr(1).Boom = 'World';
MyDSArr(1).Num  = 1;

MyDSArr(2).Boom = 'World';
MyDSArr(2).Num  = 2;

Reset MyDSArr;

Return MyDSArr(1).Boom + MyDSArr(2).Boom + %Char(MyDSArr(1).Num) + %Char(MyDSArr(2).Num);