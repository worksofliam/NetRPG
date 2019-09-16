**FREE

Dcl-F example;
dcl-s str char(2);

chain (2) example;
str = %eof(example);

chain (3) example;
str = %trimr(str) + %eof(example);

Return str;