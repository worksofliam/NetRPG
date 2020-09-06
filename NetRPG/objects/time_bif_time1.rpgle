**FREE

Dcl-s MyTime1 Time;

MyTime1 = %Time('01:23 PM':*USA);

dsply '';
dsply %char(MyTime1:*usa);
dsply %char(MyTime1);
dsply %char(t'13:23:00');

Return (MyTime1 = t'13:23:00');