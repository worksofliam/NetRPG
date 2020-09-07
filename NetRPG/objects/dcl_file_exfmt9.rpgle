**FREE

Dcl-F ex9 WorkStn;
Dcl-F employee keyed;
Dcl-s exit Ind;

exit = *Off;
dow (exit = *off);
  write HEADER;
  exfmt SEARCH;

  Select;
    When *in03;
      exit = *on;

    Other;
      chain (IID) employee;

      If %eof(employee) = *off;
        ERROR = *blank;
        CNAME = FIRSTNME + ' ' + LASTNAME;
        CEMAIL = FIRSTNME + '@me.com';

        write HEADER;
        exfmt CUSTDSP;
      else;
        ERROR = 'Cannot locate record.';
      Endif;
  Endsl;
enddo;