**FREE

Dcl-Pr printf Int(10) ExtProc('printf');
  dcl-parm format Pointer Value Options(*String);
END-PR;

dcl-s length int(3);

length = printf('Hello');
length = length + printf(' ');
length = length + printf('world');
length = length + printf('!');
dsply '';

return length;