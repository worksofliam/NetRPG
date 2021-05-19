**FREE

Dcl-F dept keyed;

dcl-s ind1 ind;
dcl-s ind2 ind;
dcl-s outtext varchar(3);

ind1 = *off;
ind2 = *off;

SetLL 'G22' dept;
ind1 = %Found(dept);
Read dept;
outtext = admrdept;

//Row does not exist.
SetLL 'ABC' dept;
ind2 = %Found(dept);

return ind1 + outtext + ind2;