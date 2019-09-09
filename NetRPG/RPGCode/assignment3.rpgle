**FREE

dcl-ds myds qualified;
  dcl-subf myint int(5);
end-ds;

myds.myint = 5;
myds.myint += 5+5;
myds.myint -= 7;
myds.myint /= 2;
myds.myint *= 3;

return myds.myint;