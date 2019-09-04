Dcl-F example;

prid = 3;
prdesc = 'My new product';

write example;

dsply '';

chain (1) example;
dsply prdesc;

chain (3) example;
dsply prdesc;

dsply '';

Return prdesc;