Dcl-S UserName Char(10) Inz(*USER);

Dsply (%TrimR(UserName) + ' is the user');
Dsply 5 + 1 + 2;
Dsply (1 + 2);
Dsply (1 - 2);

*InLR = *On;
Return;