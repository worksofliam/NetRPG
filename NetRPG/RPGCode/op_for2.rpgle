**FREE

dcl-s myint int(3);

dsply '';

for myint = 10 downto 1;
  dsply ('number: ' + %char(myint));
endfor;

dsply '';

dsply ('number: ' + %char(myint));

return myint;