using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenericData
{

    public class GenericGridReport
    {

        public GenericGridReport()
        {
            this.Columns = new List<GenericGridReportColumn>();
            this.Rows = new List<string[]>();
        }

        public List<string[]> Rows { get; private set; }

        public GenericGridReportColumn AddColumn(string columnName)
        {
            if (this.Rows.Count > 0)
            {
                throw new Exception("Can not add columns after rows have been added");
            }

            var column = new GenericGridReportColumn() { ColumnName = columnName, ColumnCount = 1 };
            column.Index = this.Columns.Count;
            this.Columns.Add(column);
            return column;
        }

        public void AddRow(params object[] values)
        {
            var row = new string[this.Columns.Count];
            for (int i = 0; i < row.Length && i < values.Length; i++)
            {
                row[i] = values[i]?.ToString();
            }

            this.Rows.Add(row);

            CalculateColumnWidth();
        }

        private void CalculateColumnWidth()
        {
            const int headerDevider = 3;

            for (var c = 0; c < this.Columns.Count; c++)
            {
                this.Columns[c].ColumnCount = (from item in this.Rows select item[c].Length).Max();
                this.Columns[c].IsNumeric = (from item in this.Rows select this.IsNumber(item[c])).Max();
                this.Columns[c].IsDate = (from item in this.Rows select this.IsDate(item[c])).Max();
                if (this.Columns[c].ColumnCount < this.Columns[c].ColumnName.Length / headerDevider)
                {
                    this.Columns[c].ColumnCount = this.Columns[c].ColumnName.Length / headerDevider;
                }
            }
        }

        private bool IsNumber(string value)
        {
            decimal dval = 0;
            return decimal.TryParse(value.Replace(" ", string.Empty).Replace(",", string.Empty), out dval);
        }

        private bool IsDate(string value)
        {
            DateTime dval = DateTime.Now;
            return DateTime.TryParse(value, out dval);
        }

        public List<GenericGridReportColumn> Columns { get; private set; }

        internal void AddColumn(string name, Type type)
        {
            var col = this.AddColumn(name);
            col.Type = type;
        }
    }

    public class GenericGridReportColumn
    {
        public string ColumnName { get; set; }

        public int ColumnCount { get; set; }

        public bool IsNumeric { get; set; }

        public Type Type { get; set; }

        public enumRepresentType RepresentType { get; set;}

        public int Index { get; internal set; }

        public string Class
        {
            get
            {
                var responces = new string[] {"Text","Number", "Date" };
                var ret = responces[0];

                if (this.IsNumeric)
                {
                    ret = responces[1];
                }

                if (this.IsDate)
                {
                    ret = responces[2];
                }

                return ret;
            }
        }

        public bool IsDate { get; set; }
    }

    public enum enumRepresentType
    {
        Data = 0,
        EmailAddress = 1,
        Subject = 2,
        Attatchment = 3,
        OutputFilePath = 4
    }
}