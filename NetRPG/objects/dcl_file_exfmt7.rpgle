**FREE

Dcl-F ex7 WorkStn;
Dcl-F cust;
Dcl-s exit Ind;

exit = *Off;
dow (exit = *off);
  write HEADER;
  exfmt SEARCH;

  Select;
    When *in03;
      exit = *on;

    When *in03 = *off;
      chain (IID) cust;

      If %eof(cust) = *off;
        ONAME = cname;
        OEMAIL = cemail;

        write HEADER;
        exfmt CUSTDSP;
      Endif;
  Endsl;
enddo;