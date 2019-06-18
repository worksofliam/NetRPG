Dcl-F ex6 WorkStn;

Dow (NAME2 = *Blank);
  exfmt OLDFMT;
Enddo;

NAME = NAME2;

dow (*in03 = *off);
  exfmt NEWFMT;
enddo;

Return NAME;