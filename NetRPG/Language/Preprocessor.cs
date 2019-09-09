using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NetRPG.Language
{
    class Preprocessor
    {
        private List<RPGLine> _Output;
        public Preprocessor()
        {
            _Output = new List<RPGLine>();
        }

        public void ReadFile(string SourcePath)
        {
            RPGLine CurrentLine;
            string[] Directive;
            bool IsFullyFree = false;
            if (File.Exists(SourcePath)) {
                
                foreach (string Line in File.ReadAllLines(SourcePath))
                {
                    //Is directive and not comment
                    if (Line.Trim() == "") {
                        continue;
                    }
                    else if (Line.Trim() == "**FREE") {
                        IsFullyFree = true;
                        continue; //Means totally free
                    }
                    else if (Line.Trim().StartsWith("//"))
                    {
                        continue;
                    }
                    else if (Line.Trim().StartsWith('/'))
                    {
                        Directive = Line.Trim().Split(' ');
                        switch (Directive[0])
                        {
                            case "/INCLUDE":
                            case "/COPY":
                                ReadFile(Directive[1]);
                                break;
                        }
                    }
                    else
                    {
                        //TODO: Remove comments
                        CurrentLine = new RPGLine(Line);
                        if (IsFullyFree) {
                            CurrentLine._Type = LineType.FullyFree;
                        } else {
                            CurrentLine.DetectLineType();
                        }

                        _Output.Add(CurrentLine);
                    }
                }
            } else {
                Error.ThrowCompileError(SourcePath + " does not exist.");
            }
        }

        public RPGLine[] GetLines() => _Output.ToArray();

        public static List<RPGToken> GetTokens(RPGLine[] Lines) {
            List<RPGToken> Tokens = new List<RPGToken>();
            RPGLex Lexer;

            foreach (RPGLine Line in Lines) {
                switch (Line._Type) {
                    case LineType.FullyFree:
                        Lexer = new RPGLex();
                        Lexer.Lex(Line._Line);
                        Tokens.AddRange(Lexer.GetTokens());
                        break;
                    case LineType.Free:
                        Lexer = new RPGLex();
                        Lexer.Lex(Line._Line.Substring(7));
                        Tokens.AddRange(Lexer.GetTokens());
                        break;
                }
            }

            return Tokens;
        }
    }
}
