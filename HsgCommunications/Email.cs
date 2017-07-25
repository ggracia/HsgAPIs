using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace HsgCommunications
{
    public class Email
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Body { get; set; }
        public bool BodyIsHTML { get; set; }
        public string From { get; set; }
        public bool EnableSSL { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
        public string AttachmentsPath { get; set; }
        public bool DeleteFilesAfterSent { get; set; }
        public string Filter { get; set; }

        private string[] attachments;

        public Email()
        {
            attachments = new string[0];
        }

        public void SendEmail()
        {
            // Creating the SMTP Client object
            var smtp = new SmtpClient(Host, Port)
            {
                EnableSsl = EnableSSL
            };

            if (!string.IsNullOrEmpty(User)) {
                var credentials = new NetworkCredential(User, Password);
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = credentials;
            }

            // Creating the email message object
            var message = new MailMessage(From, To, Subject, Body) { IsBodyHtml = BodyIsHTML };

            // Adding the attachments if any
            GetAttachments();
            foreach (var a in attachments)
            {
                var attachment = new Attachment(a);
                message.Attachments.Add(attachment);
            };

            // Sending the email
            smtp.Send(message);

            message.Attachments.Dispose();

            //Deleting files if specified.
            if (DeleteFilesAfterSent)
            {
                foreach (var f in attachments)
                {
                    FileInfo srFile = new FileInfo(f);
                    srFile.Delete();
                }
            }

        }

        public void GetAttachments()
        {
            if (string.IsNullOrEmpty(AttachmentsPath)) return;
            attachments = Directory.GetFiles(AttachmentsPath, Filter, SearchOption.TopDirectoryOnly);
        }

    }
}
