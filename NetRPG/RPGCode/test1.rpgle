Dcl-S UserName Char(10) Inz(*USER);
Dcl-S UserList Char(10) Dim(10);

Dsply ('Liam' + ' is the user');
UserName = 'Barry';
Dsply (UserName + ' is the user');
UserList(1) = 'Dave';
Dsply (UserList(1) + ' is the user');

*InLR = *On;
Return;