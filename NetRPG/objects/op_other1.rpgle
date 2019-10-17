**FREE

Dcl-S Result Int(3);

Select;
  When (1 = 2);
    Result = 1;
  When (2 = 3);
    Result = 2;
  When (3 = 4);
    Result = 4;
  Other;
    Result = 3;
Endsl;

Return Result;