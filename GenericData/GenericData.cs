using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericData
{
    public static class GenericData
    {
        public static GenericGridReport SqlToData(System.Data.Entity.DbContext context, string sSql, params string[] format)
        {
            var model = new GenericGridReport();
                try
                {
                    context.Database.Connection.Open();
                    var cmd = context.Database.Connection.CreateCommand();
                    cmd.CommandText = sSql;

                    using (var reader = cmd.ExecuteReader())
                    {
                        var schema = reader.GetSchemaTable();
                        if (schema != null)
                        {
                            foreach (System.Data.DataRow row in schema?.Rows)
                            {
                                string name = (string)row["ColumnName"];
                                Type type = (Type)row["DataType"];
                                model.AddColumn(name, type);
                            }

                            while (reader.Read())
                            {
                                var items = new object[model.Columns.Count];
                                reader.GetValues(items);
                                if (format != null)
                                {
                                    for (int i = 0; i < format.Length && i < items.Length; i++)
                                    {
                                        if (format[i] != null && items[i] != null)
                                        {
                                            items[i] = string.Format(format[i], items[i]);
                                        }
                                    }
                                }
                                model.AddRow(items);
                            }
                        }
                    }
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            return model;

        }
    }
}
