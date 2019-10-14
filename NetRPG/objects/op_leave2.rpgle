**FREE

dcl-s myint int(3);

dsply '';

for myint = 1 to 10;
  dsply ('number: ' + %char(myint));

  if (myint = 5);
    leave;
  endif;
endfor;

dsply '';

dsply ('number: ' + %char(myint));

return myint;