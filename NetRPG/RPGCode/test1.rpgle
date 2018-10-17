
Dcl-DS HelloWorld;
  Dcl-Subf MyField Char(10);
End-Ds;

Dcl-Ds QualTest Qualified;
  Dcl-Subf Random Char(10);
End-Ds;

Dcl-Ds QualDimTest Qualified Dim(10);
  Dcl-Subf Boom Char(10);
  Dcl-Subf boom2 Char(10);
End-Ds;

MyField = 'bye';
QualDimTest(5).Boom = 'yolo';
QualTest.Random = 'yolo';

Dsply QualDimTest(1).Boom;
Dsply QualDimTest(5).Boom;

If (QualDimTest(5).Boom = QualTest.Random);
  Dsply 'Hi!!';
Else;
  Dsply MyField;
Endif;

*InLR = *On;
Return;