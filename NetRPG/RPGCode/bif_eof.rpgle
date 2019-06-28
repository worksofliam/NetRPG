Dcl-F example;

dsply '';
Dow %eof(example) <> *on;
  Read example;
  Dsply prdesc;
Enddo;

Return prid;