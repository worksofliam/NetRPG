**FREE

dcl-s myint   int(3);
dcl-s counter int(3);
dcl-s anarray char(1) dim(10);

counter = 0;
dsply '';

for myint = 1 to %elem(anarray);
  dsply ('number: ' + %char(myint));
  counter += 1;
endfor;

dsply '';

dsply ('number: ' + %char(myint));

return counter;