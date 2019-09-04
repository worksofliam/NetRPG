Dcl-s MyInt Int(3) Inz(0);

MyInt = 1;
Dow (MyInt <= 10);
  MyInt = MyInt + 1;
  If (MyInt = 5);
    Dsply 'MyInt is ' + %Char(5);
  Endif;
Enddo;

Return MyInt;