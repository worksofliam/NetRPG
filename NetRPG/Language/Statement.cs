using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NetRPG.Language
{
    class Statement
    {
        public bool IsExpression = false;

        public int Line;
        public List<RPGToken> _Tokens;

        public Statement(List<RPGToken> Tokens)
        {
            _Tokens = Tokens;
        }

        public RPGToken[] GetTokens() => _Tokens.ToArray();

        public static Statement[] ParseDocument(List<RPGToken> Tokens)
        {
            List<Statement> Statements = new List<Statement>();
            List<RPGToken> CurrentStatement = new List<RPGToken>();

            foreach (RPGToken token in Tokens)
            {
                if (token.Type == RPGLex.Type.STMT_END)
                {
                    Statements.Add(new Statement(CurrentStatement));
                    CurrentStatement = new List<RPGToken>();
                }
                else
                {
                    CurrentStatement.Add(token);
                }
            }

            //Each statement has an internal line number
            for (int i = 0; i < Statements.Count; i++) {
                Statements[i].Line = i;
            }

            return Statements.ToArray();
        }

        public static RPGLex.Type[] ExpressionTypes = new[] {
            RPGLex.Type.ADD, RPGLex.Type.DIV, RPGLex.Type.EQUALS, RPGLex.Type.LESS_THAN, RPGLex.Type.LT_EQUAL, RPGLex.Type.MORE_THAN, RPGLex.Type.MT_EQUAL, RPGLex.Type.MUL, RPGLex.Type.NOT, RPGLex.Type.SUB
        };

        public static Statement[] ParseParams(List<RPGToken> Tokens)
        {
            List<Statement> Statements = new List<Statement>();
            Boolean IsExpression;
            List<RPGToken> CurrentTokens = new List<RPGToken>();
            Statement CurrentStatement;

            IsExpression = false;
            foreach (RPGToken token in Tokens)
            {
                if (token.Type == RPGLex.Type.PARMS)
                {
                    CurrentStatement = new Statement(CurrentTokens);
                    CurrentStatement.IsExpression = IsExpression;
                    Statements.Add(CurrentStatement);

                    IsExpression = false;
                    CurrentTokens = new List<RPGToken>();
                }
                else
                {
                    if (ExpressionTypes.Contains(token.Type)) {
                        IsExpression = true;
                    }

                    CurrentTokens.Add(token);
                }
            }

            if (CurrentTokens.Count > 0) {
                CurrentStatement = new Statement(CurrentTokens);
                CurrentStatement.IsExpression = IsExpression;
                Statements.Add(CurrentStatement);
            }

            return Statements.ToArray();
        }
    }
}
