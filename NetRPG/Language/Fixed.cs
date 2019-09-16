using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NetRPG.Language
{
    class RPGFixed {
        public static RPGToken[] GetTokens(string line) {
            List<RPGToken> Tokens = new List<RPGToken>();

            char[] Chars = line.ToCharArray();

            switch (char.ToUpper(Chars[4])) {
                case 'D':
                    //TODO standalone vs subf vs ds
                    Tokens.AddRange(new[] {
                        new RPGToken(RPGLex.Type.DCL), 
                        new RPGToken(RPGLex.Type.SUB), 
                        new RPGToken(RPGLex.Type.WORD_LITERAL, "S"),
                        new RPGToken(RPGLex.Type.WORD_LITERAL, new String(Chars, 7, 16)),
                    });

                    //TODO FIXED TYPE to RPGToken
                    //TODO APPEND ANY FURTHER OPTIONS
                    break;
            }

            return Tokens.ToArray();
        }
    }
}