**FREE

Dcl-F department keyed;

dcl-s output char(6) dim(2);

chain ('B01') department;
output(1) = MGRNO;

chain ('E21') department;
output(2) = MGRNO;

Return output(1) + output(2);