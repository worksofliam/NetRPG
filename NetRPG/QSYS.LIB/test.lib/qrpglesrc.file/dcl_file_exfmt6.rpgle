Dcl-F ex6 WorkStn;
Dcl-s exit Ind;

Dow (NAME2 = *Blank);
  exfmt OLDFMT;
Enddo;

NAME = NAME2;

exit = *Off;
dow (exit = *off);
  exfmt NEWFMT;
  select;
    When *in03;
      exit = *on;
    other;
      NAME = 'Nothing';
  endsl;
enddo;

Return NAME;