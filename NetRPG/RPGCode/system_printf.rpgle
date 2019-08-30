
Dcl-Pr printf Int(10) ExtProc('printf');
  dcl-parm format Pointer Value Options(*String);
END-PR;

printf('Hello');
printf(' ');
printf('world');
printf('!');