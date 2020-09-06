**FREE

Dcl-S MyTime1 TimeStamp;
Dcl-S MyTime2 TimeStamp;

//This is US format!!
MyTime1 = d'05/04/1997';
MyTime2 = MyTime1 + t'24:00:00';

Return MyTime2 = d'05/05/1997';