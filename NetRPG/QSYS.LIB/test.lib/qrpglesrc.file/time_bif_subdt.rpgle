Dcl-S MyTS Timestamp Inz(d'04/05/1997');

MyTS = MyTS + %Hours(6) + %Minutes(15);

Return %Char(%Subdt(MyTS:*Y)) + %Char(%Subdt(MyTS:*M)) + %Char(%Subdt(MyTS:*D)) + %Char(%Subdt(MyTS:*H)) + %Char(%Subdt(MyTS:*MN));  