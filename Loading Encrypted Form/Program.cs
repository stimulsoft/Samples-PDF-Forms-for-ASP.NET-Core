using Stimulsoft.Form.Items;
using Stimulsoft.Form.Pdf.Export;
using System;
using System.Diagnostics;
using System.IO;

namespace Loading_Encrypted_Form
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var form = new StiForm();
            form.LoadEncryptedForm(@"Data\SoftwareEvaluationSurvey.mdx", "secterKey");

            var exporter = new StiPdfExporter();
            var pdf = exporter.ExportForm(form);

            string path = "form.pdf";

            File.WriteAllBytes(path, pdf);

            var p = new Process
            {
                StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                }
            };
            p.Start();
        }
    }
}
