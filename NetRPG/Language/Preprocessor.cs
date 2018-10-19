using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NetRPG.Language
{
    class Preprocessor
    {
        private List<string> _Output;
        public Preprocessor()
        {
            _Output = new List<string>();
        }

        public void ReadFile(string SourcePath)
        {
            string[] Directive;
            if (File.Exists(SourcePath)) {
                
                foreach (string Line in File.ReadAllLines(SourcePath))
                {
                    //Is directive and not comment
                    if (Line.Trim().StartsWith("//"))
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
                        _Output.Add(Line);
                    }
                }
            } else {
                Error.ThrowCompileError(SourcePath + " does not exist.");
            }
        }

        public string[] GetLines() => _Output.ToArray();
    }
}
