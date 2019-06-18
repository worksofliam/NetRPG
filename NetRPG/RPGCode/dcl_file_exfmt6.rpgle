Dcl-F ex6 WorkStn;

exfmt OLDFMT;

NAME = NAME2;

dow (*in03 = *off);
  exfmt NEWFMT;
enddo;

Return NAME;