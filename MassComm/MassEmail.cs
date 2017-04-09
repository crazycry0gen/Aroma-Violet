using GenericData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MassComm
{
    public class MassEmail
    {
        private string FromEmail;
        private string Host;
        private string Password;
        private int Port;
        private string Username;

        public MassEmail(string fromEmail, string host, string password, int port, string username)
        {
            FromEmail = fromEmail;
            Host = host;
            Password = password;
            Port = port;
            Username = username;
        }

        public void Send(GenericGridReport model, EmailLayout template)
        {
            foreach(var row in model.Rows)
            {
                var newMail = new EmailLayout() { Body = template.Body, EmailTo= template.EmailTo, Subject = template.Subject};
                for(int i=0; i<model.Columns.Count; i++)
                {
                    var key = model.Columns[i].ColumnName.ElementStartEnd();
                    switch (model.Columns[i].RepresentType)
                    {
                        case enumRepresentType.Data:
                            newMail.Body = newMail.Body.Replace(key, row[i]);
                            newMail.Subject = newMail.Subject.Replace(key, row[i]);
                            break;
                        case enumRepresentType.EmailAddress:
                            newMail.EmailTo = newMail.EmailTo.Replace(key, row[i]);
                            break;
                        case enumRepresentType.Subject:
                            newMail.Subject = newMail.Subject.Replace(key, row[i]);
                            break;
                        case enumRepresentType.Attatchment:
                            newMail.Attatchments.Add(row[i]);
                            break;
                        default:
                            break;
                    }
                }
                Send(newMail);
            }
        }

        public void Send(EmailLayout mailMsg)
        {
            SmtpClient SmtpServer = new SmtpClient(this.Host);
            var mail = new MailMessage();
            mail.From = new MailAddress(this.FromEmail);
            mail.To.Add(mailMsg.EmailTo);
            mail.Subject = mailMsg.Subject;
            mail.IsBodyHtml = true;
            mail.Body = mailMsg.Body;
            SmtpServer.Port = this.Port;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(this.Username, this.Password);
            SmtpServer.EnableSsl = true;
            foreach (var att in mailMsg.Attatchments)
            {
                var info = new FileInfo(att);
                if (info.Exists)
                {
                    mail.Attachments.Add(new Attachment(att));
                }
            }
            SmtpServer.Send(mail);
        }
    }

    public class EmailLayout
    {
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Attatchments { get; set; }
        public EmailLayout()
        {
            this.Attatchments = new List<string>();
        }
    }

    public static class ElementExtensions
    {
        public static string ElementStart(this string key)
        {
            return string.Format("<{0}>");
        }
        public static string ElementEnd(this string key)
        {
            return string.Format("</{0}>");
        }
        public static string ElementStartEnd(this string key)
        {
            return string.Format("<{0}/>", key);
        }

    }
}
