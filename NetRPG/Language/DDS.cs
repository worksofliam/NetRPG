using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using NetRPG.Runtime;

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
        }

        public void ParseFile(string Location)
        {
            ParseLines(File.ReadAllLines(Location));
            Console.WriteLine("Hi");
        }

        public void ParseLines(string[] Lines)
        {
            int textcounter = 0;
            char[] chars;
            string name, len, type, dec, inout, x, y, keywords, Line = "";

            //https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_73/rzakc/rzakcmstpsnent.htm
            // try
            // {
                foreach (string TrueLine in Lines)
                {
                    Line = TrueLine.PadRight(80);
                    chars = Line.ToCharArray();
                    name = buildString(chars, 18, 10).Trim();
                    len = buildString(chars, 29, 5).Trim();
                    type = chars[34].ToString().ToUpper();
                    dec = buildString(chars, 35, 2).Trim();
                    inout = chars[37].ToString().ToUpper();
                    y = buildString(chars, 38, 3).Trim();
                    x = buildString(chars, 41, 3).Trim();
                    keywords = Line.Substring(44).Trim();

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
                                CurrentField.Position = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
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
                                HandleKeywords(keywords);
                            }
                            else
                            {
                                HandleKeywords(keywords);
                                if (CurrentField != null)
                                {
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
            }
        }
    }

  
    public class RecordInfo
    {
        public string Name;
        public FieldInfo[] Fields;
        public Boolean[] FunctionKeys;


        public Dictionary<string, string> Keywords;

        public RecordInfo(String name)
        {
            Name = name;
            Fields = new FieldInfo[0];
            FunctionKeys = new Boolean[24];
        }
    }

    public class FieldInfo
    {
        public string Name;
        public string Value;
        public DataSet dataType;
        public FieldType fieldType;
        public Point Position;

        public Dictionary<string, string> Keywords;

        public enum FieldType
        {
            Input,
            Output,
            Both,
            Const,
            Hidden
        }

        public static Color TextToColor(string Colour)
        {
            switch (Colour.ToUpper())
            {
                case "GREEN":
                    return Color.Lime;
                case "YELLOW":
                    return Color.Yellow;
                case "BLUE":
                    return Color.LightBlue;
                case "RED":
                    return Color.Red;
                case "WHITE":
                    return Color.White;
                case "TURQUOISE":
                    return Color.Turquoise;
                case "PINK":
                    return Color.Pink;

                default:
                    return Color.Green;
            }
        }


    }
}