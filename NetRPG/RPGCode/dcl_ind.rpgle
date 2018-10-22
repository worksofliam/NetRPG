Dcl-S MyInd Ind;
Dcl-S MyChar Char(3);

MyChar = '';

MyInd = '1';
MyChar = %Trim(MyChar) + MyInd;

MyInd = *Off;
MyChar = %Trim(MyChar) + MyInd;

MyInd = *On;
MyChar = %Trim(MyChar) + MyInd;

Return MyChar;