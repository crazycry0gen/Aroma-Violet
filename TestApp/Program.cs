using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericData;
using System.Net.Mail;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestDocs();
        }

        private static void TestDocs()
        {
            var doc = new MassComm.MassDocument();
            var model = new GenericGridReport();
            model.AddColumn("Destination").RepresentType = enumRepresentType.OutputFilePath;
            model.AddColumn("Tag1");
            model.AddColumn("Tag2");
            model.AddColumn("Tag3");

            model.AddRow(@"f:\temp\out1.docx", "humana hamana", "=====>>", "$%$%%^");
            model.AddRow(@"f:\temp\out2.docx", "dumdadumdum", "kjhfgjh", ">>>>>>> . . . . . ");
            model.AddRow(@"f:\temp\out3.docx", "Walleeeeeeeeee", "mc num", "woef");

            doc.GenerateDocuments(model, @"f:\temp\Template.docx");
        }

        private static void TestEmail()
        {
            var mail = new MassComm.MassEmail("c.hattingh@hotmail.com", "smtp.live.com","N0k1a!@#$&",587,"c.hattingh@hotmail.com");
            var template = new MassComm.EmailLayout() {EmailTo="<EmailAddress/>", Subject = "Hello <Name/>", Body = "Dear <strong><Name/></Strong>,<br/>Welcome to my world!" };
            var data = new GenericData.GenericGridReport();
            data.AddColumn("Name").RepresentType = enumRepresentType.Data;
            data.AddColumn("EmailAddress").RepresentType = enumRepresentType.EmailAddress;
            data.AddRow("Clifton", "c.hattingh@hotmail.com");
            data.AddRow("John", "c.hattingh@hotmail.com");
            data.AddRow("Piet", "c.hattingh@hotmail.com");
            mail.Send(data, template);
        }
    }
}
