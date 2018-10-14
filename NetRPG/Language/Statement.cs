using System;
using System.Collections.Generic;
using System.Text;

namespace NetRPG.Language
{
    class Statement
    {
        private RPGToken[] _Tokens;

        public Statement(RPGToken[] Tokens)
        {
            _Tokens = Tokens;
        }

        public RPGToken[] GetTokens() => _Tokens;

        public static Statement[] ParseDocument(List<RPGToken> Tokens)
        {
            List<Statement> Statements = new List<Statement>();
            List<RPGToken> CurrentStatement = new List<RPGToken>();

            foreach (RPGToken token in Tokens)
            {
                if (token.Type == RPGLex.Type.STMT_END)
                {
                    Statements.Add(new Statement(CurrentStatement.ToArray()));
                    CurrentStatement = new List<RPGToken>();
                }
                else
                {
                    CurrentStatement.Add(token);
                }
            }

            return Statements.ToArray();
        }
    }
}
