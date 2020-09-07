using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using NetRPG.Language;
using System.Data.Odbc;

namespace NetRPG.Runtime.Typing.Files
{
    class ODBCTable : FileT
    {
        private string _File;
        private static OdbcConnection Connection;
        private Boolean _EOF = false;
        private int _RowPointer = -1;

        public static void TestConnection() {
            if (ODBCTable.Connection == null) {
                ODBCTable.Connection = new OdbcConnection(System.Environment.GetEnvironmentVariable("CONN_STRING"));
                ODBCTable.Connection.Open();
            }
        }

        public ODBCTable(string name, string file, bool userOpen) : base(name, userOpen) {
            this.Name = name;
            this._File = file;
        
            //We'll create the connect when the user tries to define a table.
            //In the future... we should probably do this on VM bootup. Or maybe a program to do it?
            ODBCTable.TestConnection();

            if (!userOpen)
                this.Open();
        }

        public static DataSet CreateStruct(string name, Boolean qualified = false) {
            ODBCTable.TestConnection();

            DataSet Structure = new DataSet(name);
            Structure._Type = Types.Structure;
            Structure._Qualified = qualified;
            List<DataSet> subfields = new List<DataSet>();
            DataSet subfield;

            using (OdbcCommand com = new OdbcCommand(
                "select * from qsys2.syscolumns where table_name = ? and table_schema = " +
                "(select table_schema from qsys2.systables where table_name = ? limit 1)"
                , ODBCTable.Connection))
            {
                com.Parameters.AddWithValue("@var1", name.ToUpper());
                com.Parameters.AddWithValue("@var2", name.ToUpper());

                using (OdbcDataReader reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Word is from the database. Do something with it.
                        subfield = new DataSet(reader.GetString(0));
                        subfield._Type = Reader.StringToType(reader.GetString(4));
                        subfield._Length = reader.GetInt32(5);
                        subfield._Precision = (reader.IsDBNull(6) ? 0 : reader.GetInt32(6));
                        subfields.Add(subfield);
                    }
                }
            }

            Structure._Subfields = subfields;

            return Structure;
        }

        public override void Open() {
            //Open will basically do nothing..?

            this._RowPointer = -1;
        }

        public override Boolean isEOF() => this._EOF;

        private OdbcDataReader readCurrent() {
            OdbcCommand command = new OdbcCommand(
                "select * from " + this._File + " limit 1 offset ?",
                ODBCTable.Connection
            );
            command.Parameters.AddWithValue("@var1", this._RowPointer);

            return command.ExecuteReader(System.Data.CommandBehavior.SequentialAccess);
        }

        private void toStruct(DataValue Structure, OdbcDataReader statement) {
            string currentName;

            for (int i = 0; i < statement.FieldCount; i++) {
                currentName = statement.GetName(i);
                if (statement.IsDBNull(i)) {
                    Structure.GetData(currentName).DoInitialValue();
                } else {
                    Structure.GetData(currentName).Set(statement.GetValue(i));
                }
            }
        }

        public override void Read(DataValue Structure) {
            this._RowPointer += 1;

            OdbcDataReader statement = this.readCurrent();

            if (statement.Read()) {
                this._EOF = false;
                this.toStruct(Structure, statement);

            } else {
                this._EOF = true;
            }

            statement.Close();
        }

        public override void ReadPrevious(DataValue Structure) {
            this._RowPointer -= 1;

            OdbcDataReader statement = this.readCurrent();

            if (statement.Read()) {
                this._EOF = false;
                this.toStruct(Structure, statement);

            } else {
                this._EOF = true;
            }

            statement.Close();
        }

        public override void Chain(DataValue Structure, dynamic[] keys) {
            //Like above, ODBC isn'tclear if it supports read previous??

            this._EOF = true;
            this._RowPointer = -1;
            
            bool isValid = false;
            OdbcDataReader statement;

            for (int i = 0; i < keys.Length; i++)
                if (keys[i] is string)
                    keys[i] = keys[i].Trim();
            
            while (true) {
                this._RowPointer += 1;
                statement = this.readCurrent();

                if (statement.Read()) {
                    isValid = false;
                    for (int i = 0; i < keys.Length; i++)
                        if (keys[i] == (statement.GetValue(i) as dynamic))
                            isValid = true;

                    if (isValid) {
                        this.toStruct(Structure, statement);

                        this._EOF = false;
                        break;
                    }
                } else {
                    break;
                }
            }

            statement.Close();
        }
    }
}