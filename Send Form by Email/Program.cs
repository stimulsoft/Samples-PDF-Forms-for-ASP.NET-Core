using Stimulsoft.Form;
using Stimulsoft.Form.Pdf.Export;
using Stimulsoft.Form.Items;
using Stimulsoft.Form.Items.Elements;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Send_Form_by_Email
{
    internal class Program
    {
        static Hashtable Recipients = new Hashtable()
        {
            ["Alex@gmail.com"] = "Alex",
            ["Tom@gmail.com"] = "Tom",
            ["Mike@gmail.com"] = "Mike",
            ["Leo@gmail.com"] = "Leo",
            ["Genry@gmail.com"] = "Genry",
            ["Stephany@gmail.com"] = "Stephany",
            ["Kylie@gmail.com"] = "Kylie",
            ["Evstolia@gmail.com"] = "Evstolia",
            ["Pitter@gmail.com"] = "Pitter"
        };

        static void Main(string[] args)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("username", "password"),
                EnableSsl = true,
            };

            var form = new StiForm();
            form.Load(@"Data\CustomerSatisfactionSurvey.mrt");

            var label = form.GetElementByName("RecipientLabel") as StiLabelElement;

            var settings = new StiPdfExporterSettings
            {
                UsePdfA = false,
                ReadOnly = false
            };
            var pdfExporter = new StiPdfExporter(settings);

            foreach (string mail in Recipients.Keys)
            {
                label.Expression = $"Hello, {Recipients[mail]}";

                var pdf = pdfExporter.ExportForm(form);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("mail@gmail.com"),
                    Subject = "subject",
                    Body = "<h1>Hello</h1>",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(mail);

                var attachment = new Attachment(new MemoryStream(pdf), MediaTypeNames.Application.Pdf);
                mailMessage.Attachments.Add(attachment);

                smtpClient.Send(mailMessage);
            }            
        }
    }
}
