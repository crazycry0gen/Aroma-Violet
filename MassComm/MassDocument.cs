using GenericData;
using Novacode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassComm
{
    public class MassDocument
    {
        public void GenerateDocuments(GenericGridReport model, string templatePath)
        {
            var templateFile = new FileInfo(templatePath);
            if (templateFile.Exists)
            {
                var templateDoc = DocX.Load(templateFile.FullName);
                var pathColumns = (from item in model.Columns where item.RepresentType == enumRepresentType.OutputFilePath select item).ToArray();
                foreach (var row in model.Rows)
                {
                    foreach (var pathColumn in pathColumns)
                    {
                        var outFilePath = row[pathColumn.Index];
                        {
                            var outFile = new FileInfo(outFilePath);
                            if (outFile.Exists)
                            {
                                outFile.Delete();
                            }
                            var newDocument = templateDoc.Copy();

                            for (int i = 0; i < model.Columns.Count; i++)
                            {
                                if (i == pathColumn.Index)
                                {
                                    continue;
                                }
                                var key = model.Columns[i].ColumnName;
                                switch (model.Columns[i].RepresentType)
                                {
                                    case enumRepresentType.Data:
                                        foreach (var paragraph in newDocument.Paragraphs)
                                        {
                                            paragraph.ReplaceAtBookmark(row[i], key);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }

                            newDocument.SaveAs(outFile.FullName);
                        }
                    }
                }
            }
        }
    }
}