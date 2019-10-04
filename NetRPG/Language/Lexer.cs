using System;
using System.Collections.Generic;
using System.Linq;

namespace NetRPG.Language
{
    public class RPGLex
    {
        public enum Type
        {
            BLOCK, UNKNOWN, OPERATOR, STRING_LITERAL, BLOCK_OPEN, BLOCK_CLOSE, DCL, CTL, ENDDCL, OPERATION, BIF, SPECIAL, DIRECTIVE, INT_LITERAL, DOUBLE_LITERAL, WORD_LITERAL, EQUALS, ASSIGNMENT, PARMS, STMT_END, DOT, ADD, SUB, DIV, MUL, LESS_THAN, MORE_THAN, NOT, MT_EQUAL, LT_EQUAL
        }
        private string[] OPERATORS = new[] { "**=", "+=", "-=", "*=", "/=", "<>", "<=", ">=", ".", "(", ")", ";", ":", "=", "+", "/", "*", "-", "<", ">", " " };
        private char[] STRING_LITERAL = new[] { '\'' };
        private string[] BLOCK_OPEN = new[] { "(" };
        private string[] BLOCK_CLOSE = new[] { ")" };
        private Dictionary<Type, string[]> Pieces = new Dictionary<Type, string[]>
{
{Type.ASSIGNMENT, new[] {"**=", "+=", "-=", "*=", "/="}},{ Type.BLOCK_OPEN, new[] {  "(" } },{ Type.BLOCK_CLOSE, new[] {  ")" } },{Type.CTL, new[] {"CTL"}},{ Type.DCL, new[] { "DCL" } },{ Type.ENDDCL, new[] {  "END" } },{ Type.OPERATION, new[] {  "ENDIF", "ENDSL", "ENDDO", "ENDFOR", "ACQ", "ADD", "ADDDUR", "ALLOC", "ANDxx", "BEGSR", "BITOFF", "BITON", "CABxx", "CALL", "CALLB", "CALLP", "CASxx", "CAT", "CHAIN", "CHECK", "CHECKR", "CLEAR", "CLOSE", "COMMIT", "COMP", "DEALLOC", "DEFINE", "DELETE", "DIV", "DO", "DOU", "DOUxx", "DOW", "DOWxx", "DSPLY", "DUMP", "ELSE", "ELSEIF", "ENDyy", "ENDSR", "EVAL", "EVALR", "EVAL-CORR", "EXCEPT", "EXFMT", "EXSR", "EXTRCT", "FEOD", "FOR", "FORCE", "GOTO", "IF", "IFxx", "IN", "ITER", "KFLD", "KLIST", "LEAVE", "LEAVESR", "LOOKUP", "MHHZO", "MHLZO", "MLHZO", "MLLZO", "MONITOR", "MOVE", "MOVEA", "MOVEL", "MULT", "MVR", "NEXT", "OCCUR", "ON-ERROR", "ON-EXIT", "OPEN", "ORxx", "OTHER", "OUT", "PARM", "PLIST", "POST", "READ", "READC", "READE", "READP", "READPE", "REALLOC", "REL", "RESET", "RETURN", "ROLBK", "SCAN", "SELECT", "SETGT", "SETLL", "SETOFF", "SETON", "SHTDN", "SORTA", "SQRT", "SUB", "SUBDUR", "SUBST", "TAG", "TEST", "TESTB", "TESTN", "TESTZ", "TIME", "UNLOCK", "UPDATE", "WHEN", "WHENxx", "WRITE", "XFOOT", "XLATE", "XML-INTO", "XML-SAX", "Z-ADD", "Z-SUB" } },{ Type.BIF, new[] {  "/\\%\\S*/" } },{ Type.SPECIAL, new[] {  "/\\*\\S*/" } },{ Type.DIRECTIVE, new[] {  "/\\/\\S*/" } },{ Type.INT_LITERAL, new[] { "/^[-+]?\\d+$/" } },{ Type.DOUBLE_LITERAL, new[] {  @"/(?<=^| )\\d+(\\.\\d+)?(?=$| )/" } },{ Type.WORD_LITERAL, new[] {  "/.*?/" } },{ Type.EQUALS, new[] {  "=" } },{ Type.PARMS, new[] {  ":" } },{ Type.STMT_END, new[] {  ";" } },{ Type.DOT, new[] {  "." } },{ Type.ADD, new[] {  "+" } },{ Type.SUB, new[] {  "-" } },{ Type.DIV, new[] {  "/" } },{ Type.MUL, new[] {  "*" } },{ Type.LESS_THAN, new[] {  "<" } },{ Type.MORE_THAN, new[] {  ">" } },{ Type.NOT, new[] {  "<>" } },{ Type.MT_EQUAL, new[] {  ">=" } },{ Type.LT_EQUAL, new[] {  "<=" } }
};


        //***************************************************
        private RPGToken TokenList = new RPGToken(Type.BLOCK, "", 0);
        public List<RPGToken> GetTokens() => TokenList.Block;

        //***************************************************
        private int printIndex = -1;
        public void PrintBlock(List<RPGToken> Block)
        {
            printIndex++;
            foreach (RPGToken Token in Block)
            {
                Console.WriteLine("".PadRight(printIndex, '\t') + Token.Type.ToString() + " " + Token.Value + " (Line " + Token.Line.ToString() + ")");

                if (Token.Block != null)
                    PrintBlock(Token.Block);

            }
            printIndex--;
        }


        //***************************************************
        private Boolean InString = false;
        private string token = "";
        private int cIndex = 0;
        private bool IsOperator = false;
        private int CurrentLine = 1;
        public void Lex(string Text, int lineNumber = 0)
        {
            if (lineNumber > 0)
                CurrentLine = lineNumber;
                
            char c;
            TokenList.Block = new List<RPGToken>();
            while (cIndex < Text.Length)
            {
                IsOperator = false;

                if (InString == false)
                {
                    if (cIndex + 2 > Text.Length)
                    { }
                    else
                    {
                        if (Text.Substring(cIndex, 2) == Environment.NewLine)
                        {
                            CurrentLine++;
                            cIndex += 2;
                            continue;
                        }
                    }

                    foreach (string Operator in OPERATORS)
                    {
                        if (cIndex + Operator.Length > Text.Length) continue;
                        if (Text.Substring(cIndex, Operator.Length) == Operator)
                        {
                            if (Operator == "-")
                                if (cIndex + 1 < Text.Length)
                                {
                                    c = Text.Substring(cIndex+1, 1).ToCharArray()[0];
                                    if (Char.IsDigit(c))
                                    {
                                        c = Text.Substring(cIndex-1, 1).ToCharArray()[0];
                                        if (!Char.IsDigit(c)) {
                                            token += Text.Substring(cIndex, Operator.Length);
                                            cIndex += 1;
                                            break;
                                        }
                                    }
                                }

                            //Sort the old token before adding the operator
                            WorkToken();

                            //Insert new token (operator token)
                            token = Text.Substring(cIndex, Operator.Length);
                            WorkToken();

                            cIndex += Operator.Length;
                            IsOperator = true;
                            break;
                        }
                    }
                }

                if (IsOperator == false)
                {
                    c = Text.Substring(cIndex, 1).ToCharArray()[0];

                    if (STRING_LITERAL.Contains(c))
                    {
                        //This means it's end of STRING_LITERAL, and must be added to token list
                        WorkToken(InString);
                        InString = !InString;
                    }
                    else
                        token += c;


                    cIndex++;
                }
            }

            WorkToken();
        }

        private int BlockIndex = 0;
        private List<RPGToken> GetLastToken(int Direction = 0)
        {
            List<RPGToken> Result = TokenList.Block;

            BlockIndex += Direction;

            for (int levels = 0; levels < BlockIndex; levels++)
            {
                if (Result.Count() > 0)
                {
                    if (Result[Result.Count - 1].Block == null)
                        Result[Result.Count - 1].Block = new List<RPGToken>();

                    Result = Result[Result.Count - 1].Block;
                }
            }

            return Result;
        }

        public void WorkToken(Boolean stringToken = false)
        {
            string piece = token;
            token = "";

            if (piece != "")
            {
                if (stringToken == false)
                {
                    foreach (var Piece in Pieces)
                    {
                        foreach (string Value in Piece.Value)
                        {
                            if (Value.Length > 1 && Value.StartsWith("/") && Value.EndsWith("/") && !OPERATORS.Contains(piece))
                            {
                                if (System.Text.RegularExpressions.Regex.IsMatch(piece, Value.Trim('/')))
                                {
                                    GetLastToken().Add(new RPGToken(Piece.Key, piece, CurrentLine));
                                    return;
                                }
                            }
                            else
                            {
                                if (Value.ToUpper() == piece.ToUpper()) //RPG is case insensitive
                                {
                                    if (BLOCK_OPEN.Contains(piece))
                                    {
                                        GetLastToken().Add(new RPGToken(Type.BLOCK, null, CurrentLine));
                                        GetLastToken(1);
                                    }
                                    else if (BLOCK_CLOSE.Contains(piece))
                                    {
                                        GetLastToken(-1);
                                    }
                                    else
                                    {
                                        GetLastToken().Add(new RPGToken(Piece.Key, piece, CurrentLine));
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                    GetLastToken().Add(new RPGToken(Type.STRING_LITERAL, piece, CurrentLine));
            }
            else
                if (stringToken == true)
                GetLastToken().Add(new RPGToken(Type.STRING_LITERAL, piece, CurrentLine));
        }
    }
    public class RPGToken
    {
        public List<RPGToken> Block;
        public RPGLex.Type Type;
        public string Value;
        public int Line;

        public RPGToken(RPGLex.Type type, string value = "", int line = 0)
        {
            Type = type;
            Value = value;
            Line = line;
            Block = null;
        }
    }
}
