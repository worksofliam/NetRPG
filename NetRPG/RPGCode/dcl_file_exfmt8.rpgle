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
      DisplayCust(IID);
  Endsl;
enddo;

Dcl-Proc DisplayCust;
  Dcl-Pi *n;
    Dcl-Parm CustID Int(10);
  End-Pi;

  chain (CustID) cust;

  If %eof(cust) = *off;
    ONAME = cname;
    OEMAIL = cemail;

    write HEADER;
    exfmt CUSTDSP;
  Endif;
End-Proc;