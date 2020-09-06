Dcl-F department keyed;
Dcl-F act keyed;

chain ('B01') department;
chain (60) act;

Return %trim(MGRNO) + %trim(ACTKWD);