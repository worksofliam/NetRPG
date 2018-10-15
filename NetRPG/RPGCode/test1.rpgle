Dcl-S UserName Char(10) Inz(*USER);

Dsply (%TrimR(UserName) + ' is the user');

If (1 = 2);
  Dsply 'yes';
Elseif (2 = 3);
  Dsply 'no';
Elseif (3 = 4);
  Dsply 'Maybe';
Else;
  Dsply 'No defo';
Endif;
Dsply (1 + 2);
Dsply (1 - 2);

*InLR = *On;
Return;