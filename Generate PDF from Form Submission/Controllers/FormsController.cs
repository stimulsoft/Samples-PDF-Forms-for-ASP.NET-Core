using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Form.Items;
using Stimulsoft.Form;
using Stimulsoft.Form.Items.Elements;
using Stimulsoft.Form.Web;
using System.Collections;
using System.Text;
using System;
using Microsoft.AspNetCore.Http;
using Stimulsoft.Form.Submission;
using Stimulsoft.Form.Helpers;
using System.Diagnostics;
using Stimulsoft.Form.Pdf.Export;

namespace Generate_PDF_from_Form_Submission.Controllers
{
    [Controller]
    public class FormsController : Controller
    {
        private static string FormPath = @"Data\SchoolApplication.mrt";
        private static StiForm resultForm;
        private static StiPdfFormSubmission submission;
        private static bool dataChanged = false;

        static FormsController()
        {
            // How to Activate
            //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnO...";
            //Stimulsoft.Base.StiLicense.LoadFromFile("license.key");
            //Stimulsoft.Base.StiLicense.LoadFromStream(stream);
        }

        [HttpPost]
        public IActionResult Action()
        {
            try
            {
                var data = JObject.Parse(this.HttpContext.Request.Form["data"]);
                var action = data["action"].ToString();
                switch (action)
                {
                    case "Initialize":
                        resultForm = null;
                        var initData = StiWebForm.Initialize(data, Initialize(data));                        
                        return Json(initData.Content);

                    case "SaveForm":
                        var form = new StiForm();
                        form.Load(Convert.FromBase64String(data["form"].ToString()));
                        form.Save(FormPath);
                        return Json(form);

                    case "IsDataChanged":
                        return Json(dataChanged);

                    default:
                        var result = StiWebForm.ProcessRequest(data);
                        return result.ContentType switch
                        {
                            "application/pdf" => new FileContentResult(result.Content as byte[], result.ContentType),

                            _ => Json(result.Content),
                        };
                }
            }
            catch (Exception e)
            {
                return new ContentResult()
                {
                    Content = e.Message,
                    ContentType = "text/plain"
                };
            }
        }

        public IActionResult PostForm()
        {
            HttpRequest request = this.HttpContext.Request;
            string requestBodyString;
            try
            {
                request.EnableBuffering();
                byte[] buffer = new byte[Convert.ToInt32(request.ContentLength)];
                request.Body.ReadAsync(buffer, 0, buffer.Length);
                requestBodyString = Encoding.UTF8.GetString(buffer);

                submission = new StiPdfFormSubmission();
                submission.ParseXFDF(buffer);

                dataChanged = true;
            }
            finally
            {
                request.Body.Position = 0;
            }

            return new ContentResult(); 
        }

        public IActionResult GetPdf()
        {
            var form = new StiForm();
            form.Load(FormPath);

            if (submission != null)
            {
                var data = submission.ConvertData(form);
                StiFormFillHelper.FillForm(form, data);
            }
            (form.GetElementByName("Button") as StiButtonElement).Visible = false;
            
            StiPdfExporter exporter = new StiPdfExporter();
            var pdf = exporter.ExportForm(form);

            dataChanged = false;

            return new FileContentResult(pdf, "application/pdf"); 
        }

        private Hashtable Initialize(JObject data)
        {
            var form = new StiForm();
            form.Load(FormPath);
           
            var options = new Hashtable
            {
                ["form"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(form.SaveToString()))
            };

            return options;
        }


    }
}
