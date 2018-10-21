Dcl-Ds MyDSArr Qualified Dim(2);
  Dcl-Subf Boom Char(10);
  Dcl-Subf Num  Int(3);
End-Ds;

MyDSArr(1).Boom = 'Hello';
MyDSArr(1).Num  = 1;

MyDSArr(2).Boom = 'World';
MyDSArr(2).Num  = 2;

Return MyDSArr(1).Boom + MyDSArr(2).Boom + %Char(MyDSArr(1).Num + MyDSArr(2).Num);