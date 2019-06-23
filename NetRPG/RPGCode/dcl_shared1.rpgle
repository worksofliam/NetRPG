dcl-ds ds1;
  dcl-subf myname char(10);
end-ds;

dcl-ds ds2;
  dcl-subf myname char(10);
end-ds;

myname = 'Hi';

dsply ds1.myname;
dsply ds2.myname;

return ds2.myname;