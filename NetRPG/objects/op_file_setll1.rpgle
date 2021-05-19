**FREE

Dcl-F dept keyed;

dcl-s ind1 ind;
dcl-s ind2 ind;
dcl-s ind3 ind;

ind1 = *off;
ind2 = *off;
ind3 = *off;

//Find a row
SetLL 'G22' dept;
ind1 = %Found(dept);

//But can't find a row before it
SetLL 'D11' dept;
ind2 = %Found(dept);

//Unless we reset it.
SetLL *START dept;
SetLL 'D11' dept;
ind3 = %Found(dept);

return ind1 + ind2 + ind3;