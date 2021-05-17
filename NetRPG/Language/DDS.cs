using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NetRPG.Runtime;
using Terminal.Gui;

namespace NetRPG.Language
{
    public class DisplayParse
    {
        private Dictionary<string, RecordInfo> Formats;
        private RecordInfo CurrentRecord;

        private List<FieldInfo> CurrentFields;
        private FieldInfo CurrentField;

        public DisplayParse()
        {
            Formats = new Dictionary<string, RecordInfo>();

            CurrentRecord = new RecordInfo("GLOBAL");
            CurrentRecord.Keywords = new Dictionary<string, string>();
        }

        public void ParseFile(string Location)
        {
            ParseLines(File.ReadAllLines(Location));
        }

        private void ParseLines(string[] Lines)
        {
            int textcounter = 0;
            char[] chars;
            string conditionals, name, len, type, dec, inout, x, y, keywords, Line = "";

            //https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_73/rzakc/rzakcmstpsnent.htm
            // try
            // {
                foreach (string TrueLine in Lines)
                {
                    Line = TrueLine.PadRight(80);
                    chars = Line.ToCharArray();
                    conditionals = buildString(chars, 6, 10).ToUpper();
                    name = buildString(chars, 18, 10).Trim();
                    len = buildString(chars, 29, 5).Trim();
                    type = chars[34].ToString().ToUpper();
                    dec = buildString(chars, 35, 2).Trim();
                    inout = chars[37].ToString().ToUpper();
                    y = buildString(chars, 38, 3).Trim();
                    x = buildString(chars, 41, 3).Trim();
                    keywords = Line.Substring(44).Trim();

                    if (chars[6] == '*') {
                        continue;
                    }

                    switch (chars[16])
                    {
                        case 'R':
                            if (CurrentField != null) CurrentFields.Add(CurrentField);
                            if (CurrentFields != null) CurrentRecord.Fields = CurrentFields.ToArray();
                            if (CurrentRecord != null) Formats.Add(CurrentRecord.Name, CurrentRecord);

                            CurrentRecord = new RecordInfo(name);
                            CurrentRecord.Keywords = new Dictionary<string, string>();
                            CurrentFields = new List<FieldInfo>();
                            CurrentField = null;
                            HandleKeywords(keywords);
                            break;

                        case ' ':
                            if ((x != "" && y != "") || inout == "H")
                            {
                                if (CurrentField != null)
                                    CurrentFields.Add(CurrentField);

                                if (inout == "H")
                                {
                                    x = "0"; y = "0";
                                }

                                CurrentField = new FieldInfo();
                                CurrentField.Position = new System.Drawing.Point(Convert.ToInt32(x), Convert.ToInt32(y));
                            }
                            if (name != "")
                            {
                                CurrentField.Name = name;
                                CurrentField.Value = "";
                                CurrentField.Keywords = new Dictionary<string, string>();
                                switch (inout)
                                {
                                    case "I":
                                        CurrentField.fieldType = FieldInfo.FieldType.Input;
                                        break;
                                    case "B":
                                        CurrentField.fieldType = FieldInfo.FieldType.Both;
                                        break;
                                    case "H":
                                        CurrentField.fieldType = FieldInfo.FieldType.Hidden;
                                        break;
                                    case " ":
                                    case "O":
                                        CurrentField.fieldType = FieldInfo.FieldType.Output;
                                        break;
                                }

                                CurrentField.dataType = new DataSet(CurrentField.Name);
                                CurrentField.dataType._Length = Convert.ToInt32(len);
                                if (dec != "") CurrentField.dataType._Precision = Convert.ToInt32(dec);
                                switch (type.ToUpper())
                                {
                                    case "":
                                      //TODO use length and dec to determine type
                                      // if decimals is blank, then is a character field
                                      if  (dec == "") 
                                        CurrentField.dataType._Type = Types.Character;
                                      break;
                                      
                                    case "D":
                                    case "S":
                                        CurrentField.dataType._Type = Types.FixedDecimal;
                                        break;
                                    default:
                                        CurrentField.dataType._Type = Types.Character;
                                        break;
                                }
                                HandleConditionals(conditionals);
                                HandleKeywords(keywords);
                            }
                            else
                            {
                                HandleKeywords(keywords);
                                if (CurrentField != null)
                                {
                                    HandleConditionals(conditionals);
                                    if (CurrentField.Name == null)
                                    {
                                        textcounter++;
                                        CurrentField.Name = "TEXT" + textcounter.ToString();
                                        CurrentField.dataType = new DataSet(CurrentField.Name);
                                        if (CurrentField.Value == null) CurrentField.Value = "";
                                        CurrentField.dataType._Length = CurrentField.Value.Length;
                                        CurrentField.fieldType = FieldInfo.FieldType.Const;
                                    }
                                }
                            }
                            break;
                    }
                }
            // }
            // catch (Exception e)
            // {
            //     Error.ThrowCompileError("Unable to read parse display file.");
            // }

            if (CurrentField != null) CurrentFields.Add(CurrentField);
            if (CurrentFields != null) CurrentRecord.Fields = CurrentFields.ToArray();
            if (CurrentRecord != null) Formats.Add(CurrentRecord.Name, CurrentRecord);

        }

        public Dictionary<string, RecordInfo> GetRecordFormats()
        {
            return Formats;
        }

        private string buildString(char[] array, int from, int len)
        {
            string outp = "";
            for (var i = from; i < from + len; i++)
            {
                outp += array[i];
            }
            return outp;
        }

        private void HandleKeywords(string Keywords)
        {
            if (Keywords == "") return;
            
            if (Keywords.StartsWith("'") && Keywords.EndsWith("'"))
            {
                CurrentField.Value = Keywords.Trim('\'');
                return;
            }
            if (Keywords.Contains("(") && Keywords.EndsWith(")"))
            {
                int midIndex = Keywords.IndexOf('(');
                string option = Keywords.Substring(0, midIndex).ToUpper();
                string value = Keywords.Substring(midIndex + 1);
                value = value.Substring(0, value.Length - 1);

                if (CurrentField != null)
                    CurrentField.Keywords.Add(option, value);
                else
                    CurrentRecord.Keywords.Add(option, value);
            } else {
                foreach (string keyword in Keywords.Split(' ')) {
                    if (CurrentField != null)
                        CurrentField.Keywords.Add(keyword, "");
                    else
                        CurrentRecord.Keywords.Add(keyword, "");
                }
            }
        }

        private void HandleConditionals(string Conditionals) {
            if (Conditionals.Trim() == "") return;

            //TODO: something with condition
            string condition = Conditionals.Substring(0, 1); //A (and) or O (or)

            string current = "";
            bool negate = false;
            int indicator = 0;

            int cIndex = 1;

            while (cIndex <= 7) {
                current = Conditionals.Substring(cIndex, 3);

                if (current.Trim() != "") {
                    negate = (Conditionals.Substring(cIndex, 1) == "N");
                    indicator = int.Parse(Conditionals.Substring(cIndex+1, 2));

                    CurrentField.Conditionals.Add(new Conditional {indicator = indicator, negate = negate});
                }
                
                cIndex += 3;
            }
        }

        public static Key IntToKey(int value) {
            switch (value) {
                case 1: return Key.F1;
                case 2: return Key.F2;
                case 3: return Key.F3;
                case 4: return Key.F4;
                case 5: return Key.F5;
                case 6: return Key.F6;
                case 7: return Key.F7;
                case 8: return Key.F8;
                case 9: return Key.F9;
                case 10: return Key.F10;
                case 12: return Key.Esc;
            }

            return Key.End;
        }
    }

  
    public class RecordInfo
    {
        public string Name;
        public FieldInfo[] Fields;
        public Dictionary<Key, int> Function;


        public Dictionary<string, string> Keywords;

        public RecordInfo(String name)
        {
            Name = name;
            Fields = new FieldInfo[0];
            Function = new Dictionary<Key, int>();
        }
    }

    public class FieldInfo
    {
        public string Name;
        public string Value;
        public DataSet dataType;
        public FieldType fieldType;
        public System.Drawing.Point Position;

        public List<Conditional> Conditionals = new List<Conditional>();

        public Dictionary<string, string> Keywords = new Dictionary<string, string>();

        public enum FieldType
        {
            Input,
            Output,
            Both,
            Const,
            Hidden
        }

    }

    public class Conditional {
        public Boolean negate = false;
        public int indicator;
    }
}