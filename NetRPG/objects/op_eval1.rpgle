**FREE

Dcl-s MyInd Ind;
Dcl-S MyInt Int(3) Inz(0);

MyInd = *on;

If (MyInd);
  eval MyInt = 4 + 6;
Endif;

Return MyInt;