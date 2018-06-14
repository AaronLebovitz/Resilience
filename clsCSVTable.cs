using System;
using System.IO;
using System.Data;

namespace ResilienceClasses
{
    public class clsCSVTable
    {
        private string strPath;
        private bool bLoaded = false;
        private string[] Columns;
        private System.Collections.Generic.List<string> Values = new System.Collections.Generic.List<string>();

        public clsCSVTable(string path)
        {
            this.strPath = path;
            this.bLoaded = this._Load();
        }

        private bool _Load()
        {
            string line;
            StreamReader file;
            try
            {
                file = new StreamReader(this.strPath);
                // load columns
                if ((line = file.ReadLine()) != null)
                {
                    this.Columns = System.Text.RegularExpressions.Regex.Split(line, ",");
                }
                else
                {
                    return false;
                }
                // load values
                while ((line = file.ReadLine()) != null)
                {
                    this.Values.Add(line);
                }
                file.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SaveAs(string path)
        {
            StreamWriter file;
            try
            {
                file = new StreamWriter(path);
                // write columns
                file.WriteLine(this._ColumnsCSV());
                // write values
                for (int i = 0; i < this.Values.Count; i++)
                {
                    file.WriteLine(this.Values[i]);
                }
                file.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Save()
        {
            return this.SaveAs(this.strPath);
        }

        private string _ColumnsCSV()
        {
            string str = Columns[0];
            for (int i = 1; i < Columns.Length; i++)
            {
                str += "," + Columns[i];
            }
            return str;
        }

        public bool New(string[] values)
        {
            if (values.Length == this.Width() - 1)
            {
                string strValues;
                strValues = (this.Length()+1).ToString();
                for (int i = 0; i < values.Length; i++)
                {
                    strValues += "," + values[i];
                }
                Values.Add(strValues);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Length()
        {
            return Values.Count;
        }

        public int Width()
        {
            return Columns.Length;
        }

        public string Value(int row, int col)
        {
            if ((row < this.Length()) && (col < this.Width()))
            {
                return System.Text.RegularExpressions.Regex.Split(Values[row], ",")[col];
            }
            else
            {
                return null;
            }
        }

        public string Value(int row, string columnName)
        {
            return this.Value(row, this._ColumnIndex(columnName));
        }

        public bool Update(int row, int col, string val)
        {
            if ((row > 0) && (row < this.Length()) &&(col>0)&&(col<this.Width()))
            {
                string strNewrecord = row.ToString();
                for (int i = 1; i < this.Width(); i++)
                {
                    strNewrecord += ",";
                    if (i == col)
                    {
                        strNewrecord += val;
                    }
                    else
                    {
                        strNewrecord += this.Value(row, i);
                    }
                }
                this.Values[row] = strNewrecord;
                return true;
            }
            else
            {
                return false;
            }
        }

        private int _ColumnIndex(string columnName)
        {
            int iIndex = -1;
            for (int i = 0; i < this.Columns.Length; i++)
            {
                if (this.Columns[i] == columnName)
                {
                    iIndex = i;
                }
            }
            return iIndex;
        }

        public string ColumnNames()
        {
            return this._ColumnsCSV();
        }

    }
}
