Dcl-S UserName Char(10) Inz(*USER);
Dcl-S UserList Char(10) Dim(10);

Dsply (%TrimR(UserName) + ' is the user');

Dsply MyDS.Name;
Dsply UserList(1);
Dsply getUserList(1:'Email address');

*InLR = *On;
Return;