**FREE

Dcl-F employee;
Dcl-F dept;

dcl-s counted int(5);

Read employee;
Dow %found(employee);
  counted += 1;
  Read employee;
Enddo;

Read dept;
Dow %found(dept);
  counted += 1;
  Read dept;
Enddo;

Dsply %char(counted);

Return counted;