namespace NetRPG.Language
{
    enum LineType {
        FullyFree, Free, Fixed, Ignore
    } 
    class RPGLine
    {
        public string _Line;
        public LineType _Type;
        public RPGLine(string Line) {
            this._Line = Line;
        }

        public void DetectLineType() {
            if (this._Line.Length == 0)
                return;

            if (this._Line.Length <= 4) 
                return;

            char[] Chars = this._Line.ToCharArray();

            if (Chars[5] == '*') {
                this._Type = LineType.Ignore;
            } else {
                if (char.IsLetter(Chars[4])) {
                    this._Type = LineType.Fixed;
                } else {
                    this._Type = LineType.Free;
                }
            }
        }
    }
}