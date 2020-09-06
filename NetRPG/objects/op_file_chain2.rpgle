**FREE

Dcl-F example keyed;

dcl-s output char(10) dim(2);

chain (2) example;
output(1) = prdesc;

chain (1) example;
output(2) = prdesc;

Return output(1) + output(2);