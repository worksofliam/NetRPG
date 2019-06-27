Dcl-F ex9 WorkStn;
Dcl-F cust2;
Dcl-s exit Ind;

exit = *Off;
dow (exit = *off);
  write HEADER;
  exfmt SEARCH;

  Select;
    When *in03;
      exit = *on;

    Other;
      chain (IID) cust2;

      If %eof(cust2) = *off;
        write HEADER;
        exfmt CUSTDSP;
      Endif;
  Endsl;
enddo;