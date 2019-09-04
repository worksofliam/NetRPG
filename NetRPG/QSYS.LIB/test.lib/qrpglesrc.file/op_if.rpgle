Dcl-s MyInd Ind;
Dcl-S MyInt Int(3) Inz(0);

MyInd = (1 = 1);

If (MyInd);
  MyInt = MyInt + 1;
Endif;

If MyInd;
  MyInt = MyInt + 1;
Endif;

If MyInd = *On;
  MyInt = MyInt + 1;
Endif;

Return MyInt;