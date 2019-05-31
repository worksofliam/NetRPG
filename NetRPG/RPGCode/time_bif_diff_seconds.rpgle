Dcl-S MyTime1 TimeStamp;
Dcl-S MyTime2 TimeStamp;

//This is US format!!
MyTime1 = d'05/04/1997';
MyTime2 = MyTime1 + %Days(1);

Return %Diff(MyTime1:MyTime2:*Seconds);