using System;
using System.IO;
using NetRPG.Language;

namespace NetRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            string SourcePath = Path.Combine(Environment.CurrentDirectory, "RPGCode", "test1.rpgle");
            Console.WriteLine("Hello World: " + Environment.CurrentDirectory);
            Preprocessor prep = new Preprocessor();

            prep.ReadFile(SourcePath);

            RPGLex lexer = new RPGLex();
            lexer.Lex(String.Join(Environment.NewLine, prep.GetLines()));

            Statement[] Statements = Statement.ParseDocument(lexer.GetTokens());

            Console.WriteLine(Statements.Length);

            Console.ReadKey();
        }
    }
}
