
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

// Dsply QualDimTest(1).Boom;
// Dsply QualDimTest(5).Boom;

// If (QualDimTest(1).Boom <> QualDimTest(5).Boom);
//   Dsply 'Correct';
// Endif;

QualDimTest(1).boom2 = 'Hi';

// If (QualDimTest(5).Boom = QualTest.Random);
//   Dsply 'Hi!!';
// Else;
//   Dsply MyField;
// Endif;

// If (1 = 1);

//   If (2 = 3);

//     If (3 = 3);
//       Dsply 'hi1';
//     Endif;
//   else;
//     If (1 = 1);
//       Dsply 'hi2';
//     Endif;
//   Endif;

// Endif;

Select;
  When (QualDimTest(1).Boom <> QualDimTest(5).Boom);
    Dsply 'Hello';
  When (QualTest.Random = 'swag');
    Dsply QualTest.Random;
  When QualDimTest(1).boom2 = 'Hi';
    Select;
      When (MyField = 'no');
        Dsply 'hhi';
    Endsl;
  When MyField = 'bye';
    Dsply 'Do a thing';
Endsl;

*InLR = *On;
Return;