**FREE

dcl-s myint   int(3);
dcl-s counter int(3);

counter = 0;
dsply '';

for myint = 1 by 2 to 10;
  dsply ('number: ' + %char(myint));
  counter += 1;
endfor;

dsply '';

dsply ('number: ' + %char(myint));

return counter;