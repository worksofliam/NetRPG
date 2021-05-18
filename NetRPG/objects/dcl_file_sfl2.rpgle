**FREE

Dcl-F sfl1 WorkStn;

dcl-s index int(3);
dcl-s length int(3);
dcl-s files char(30) dim(10);

dcl-s max_per_page int(3) inz(5);
dcl-s rrn int(3);

files(1) = 'home.rpgle';
files(2) = 'other.rpgle';
files(3) = 'bf.rpgle';
files(4) = 'swag.rpgle';
files(5) = 'goals.rpgle';
files(6) = 'a.rpgle';
files(7) = 'b.rpgle';
files(8) = 'c.rpgle';
files(9) = 'd.rpgle';
files(10) = 'e.rpgle';

length = %elem(files);
rrn = 0;

dow (*in03 = *off);

  //Clear subfile
  *in85 = *on;
  write SFLDTA;

  for index = 1 to max_per_page;
    if ((rrn+index) <= length);
      clear @1SEL;
      @XFILE = files(rrn+index);
      write SFLDTA;
    endif;
  endfor;

  write HEADER_FMT;
  write FOOTER_FMT;
  exfmt SFLCTL;

  *in05 = *off;
  *in06 = *off;

  ReadC SFLDTA;
  If %Found(SFLDTA);
    TEXTA = @XFILE;
    *in06 = *On;
  Endif;

  //Pagedown
  if (*in98);
    rrn += 5;
  endif;

  //Pageup
  if (*in99);
    rrn -= 5;
  endif;

  if (rrn < 0);
    rrn = 0;
  endif;

  if (rrn >= length);
    rrn = length - 5;
    *in05 = *on;
  endif;

enddo;

return;