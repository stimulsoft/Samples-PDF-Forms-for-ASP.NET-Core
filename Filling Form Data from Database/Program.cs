using Stimulsoft.Form;
using Stimulsoft.Form.Pdf.Export;
using Stimulsoft.Form.Items;
using Stimulsoft.Form.Items.Elements;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using MySqlConnector;
using System.Threading.Tasks;

namespace Filling_Form_Data_from_Database
{
    internal class Program
    {
        static async Task Main(string[] args)
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

            var pdfExporter = new StiPdfExporter();

            using var connection = new MySqlConnection("Server=YOURSERVER;User ID=YOURUSERID;Password=YOURPASSWORD;Database=YOURDATABASE");

            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT mail, recipient FROM table;", connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var mail = reader.GetString("mail");
                var recipient = reader.GetString("recipient");

                label.Expression = $"Hello, {recipient}";

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
