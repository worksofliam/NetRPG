**FREE

Dcl-F ex5 WorkStn;

*in05 = *off;

Dow *on;
  write HDRFTR;
  exfmt INPUT;

  *in05 = (NAME = 'abcd');

  if (NAME = 'exit');
    return;
  endif;
enddo;

Return NAME;