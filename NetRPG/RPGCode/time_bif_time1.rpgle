Dcl-s MyTime1 Time;

MyTime1 = %Time('01:23 PM':*USA);

Return (MyTime1 = t'13:23:00');