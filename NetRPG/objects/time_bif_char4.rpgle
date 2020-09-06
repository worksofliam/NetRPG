**FREE

Dcl-S MyTimeStamp Date;

//Us date format....
Dsply '';
MyTimeStamp = d'04/05/1997';
Dsply %Char(MyTimeStamp);
MyTimeStamp = MyTimeStamp + %hours(12);

Return %Char(MyTimeStamp:*ISO);