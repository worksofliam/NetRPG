Dcl-S UserName Char(10) Inz('Barry');
Dcl-S UserList Char(10) Dim(10);

UserList(1) = 'Dave';

if UserList(1) = 'Dave';
  If (UserName = 'Barry');
    Dsply ('yolo');
  Else;
    Dsply ('No no');
  Endif;

  Dsply UserName + ' is the name';

  Dsply 'hi';
Endif;

*InLR = *On;
Return;