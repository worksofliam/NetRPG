**FREE

Dcl-F department;
dcl-s count int(3);

count = 0;

dsply '';

read department;
Dow %eof(department) = *off;
  dsply DEPTNAME;
  count = count + 1;
  read department;
enddo;

Return count;