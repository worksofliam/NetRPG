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

        private OdbcDataReader _Statement;

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
                        string word = reader.GetString(0);
                        // Word is from the database. Do something with it.
                        subfield = new DataSet(reader.GetString(0));
                        subfield._Type = Reader.StringToType(reader.GetString(4));
                        subfield._Length = reader.GetInt32(5);
                        subfield._Precision = (reader.IsDBNull(6) ? 0 : reader.GetInt32(5));
                        subfields.Add(subfield);
                    }
                }
            }

            Structure._Subfields = subfields;

            return Structure;
        }

        public override void Open() {
            OdbcCommand command = new OdbcCommand(
                "select * from " + this._File,
                ODBCTable.Connection
            );

            this._Statement = command.ExecuteReader(System.Data.CommandBehavior.SequentialAccess);
        }

        public override Boolean isEOF() => this._EOF;

        public override void Read(DataValue Structure) {
            string currentName;
            if (this._Statement.Read()) {
                this._EOF = false;

                for (int i = 0; i < this._Statement.FieldCount; i++) {
                    currentName = this._Statement.GetName(i);
                    if (!this._Statement.IsDBNull(i)) {
                        Structure.GetData(currentName).Set(this._Statement.GetValue(i));
                    }
                }

            } else {
                this._EOF = true;
            }
        }

        public override void ReadPrevious(DataValue Structure) {
            //Not sure if ODBC supports read previous? Might have to implement something here....
            // this._RowPointer -= 1;

            // if (this._RowPointer >= 0 && this._RowPointer < this._Data.Count()) {
            //     this._EOF = false;

            //     foreach (string varName in this._Data[this._RowPointer].Keys.ToArray()) {
            //         Structure.GetData(varName).Set(this._Data[this._RowPointer][varName]);
            //     }

            // } else {
            //     this._EOF = true;
            // }
        }

        public override void Chain(DataValue Structure, dynamic[] keys) {
            //Like above, ODBC isn'tclear if it supports read previous??
            // this._EOF = true;

            // this._RowPointer = 0;

            // while (this._RowPointer >= 0 && this._RowPointer < this._Data.Count()) {

            //     for (var i = 0; i < keys.Length; i++) {
            //         if (keys[i] != this._Data[this._RowPointer].ElementAt(i).Value) {
            //             continue;
            //         }

            //         foreach (string varName in this._Data[this._RowPointer].Keys.ToArray()) {
            //             Structure.GetData(varName).Set(this._Data[this._RowPointer][varName]);
            //         }

            //         this._EOF = false;
            //         return;
            //     }

            //     this._RowPointer += 1;

            // }
        }
    }
}