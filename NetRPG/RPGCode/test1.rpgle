
Dcl-DS HelloWorld;
  Dcl-Subf MyField Char(10);
End-Ds;

Dcl-Ds QualTest Qualified;
  Dcl-Subf Random Char(10);
End-Ds;

Dcl-Ds QualDimTest Qualified Dim(10);
  Dcl-Subf Boom Char(10);
  Dcl-Subf boom2 Char(10);
  Dcl-Subf boomint Int(5);
End-Ds;

Dcl-S MyVar Char(50);
Dcl-S MyInt Int(5);

MyField = '';
QualDimTest(5).Boom = 'yolo';
QualTest.Random = 'yolo';

MyInt = 5;
MyInt = MyInt + 5;

// Dsply QualDimTest(1).Boom;
// Dsply QualDimTest(5).Boom;

// If (QualDimTest(1).Boom <> QualDimTest(5).Boom);
//   Dsply 'Correct';
// Endif;

QualDimTest(1).boom2 = 'Person';
QualDimTest(3).boomint = 25234;

Dsply '"You are ' + %Trim(QualDimTest(1).boom2) + '"';

QualDimTest(2).boom2 = '55.56634';
Dsply %Char(%Dec(QualDimTest(2).boom2));
Dsply %Char(%DecPos(%Dec(QualDimTest(2).boom2)));

Dsply %EditC(1000 * 12:'B':'$');
Dsply %Char(MyInt);
MyInt = 3;
Dsply %Char(QualDimTest(MyInt).boomint);

*InLR = *On;
Return;