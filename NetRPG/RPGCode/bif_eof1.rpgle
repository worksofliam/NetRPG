**FREE

Dcl-F example;

dsply '';
Read example;

Dow %eof(example) <> *on;
  Dsply prdesc;
  Read example;
Enddo;

Return prid;