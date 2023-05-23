using Stimulsoft.Form.Pdf.Export;
using Stimulsoft.Form.Items;
using System;
using System.Diagnostics;
using System.IO;

namespace Loading_Form_from_Resource
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var form = new StiForm();
            form.LoadFromResource(Type.GetType("Loading_Form_from_Resource.Program").Assembly, "Loading_Form_from_Resource.Data.SoftwareEvaluationSurvey.mrt");

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
