using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Form;
using Stimulsoft.Form.Export.Pdf;
using Stimulsoft.Form.Items;
using Stimulsoft.Form.Items.Elements;
using Stimulsoft.Form.Localization;
using Stimulsoft.Form.Result;
using Stimulsoft.Form.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NetCoreAngularApplication.Controllers
{
    [Controller]
    public class FormsController : Controller
    {        
        private static List<Hashtable> Submits = new List<Hashtable>();        

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
                        var initData = StiWebForm.Initialize(data, Initialize(data));
                        return Json(initData.Content);

                    case "GetFonts":
                        if (data["fonts"] != null &&  (data["fonts"] as JArray).Count> 0 && (data["fonts"] as JArray)[0]["fontFamily"].ToString() == "Mrs. Monster")
                        {
                            List<Hashtable> fontResources = new List<Hashtable>();
                            
                            /*Hashtable fontResourceItem = new Hashtable();
                            var content = System.IO.File.ReadAllBytes("Fonts/mrsmonster.ttf");
                            fontResourceItem["contentForCss"] = String.Format("data:{0};base64,{1}", "application/x-font-ttf", Convert.ToBase64String(content));
                            fontResourceItem["originalFontFamily"] = "Mrs. Monster";
                            fontResources.Add(fontResourceItem);*/

                            return Json(fontResources);
                        }
                        return Json("{}");

                    default:
                        var result = StiWebForm.ProcessRequest(data);
                        return result.ContentType switch
                        {
                            "application/pdf" => new FileContentResult(result.Content as byte[], result.ContentType),

                            _ => Json(result.Content),
                        };
                }
            }catch (Exception e)
            {
                return new ContentResult()
                {
                    Content = e.Message,
                    ContentType = "text/plain"
                };    
            }
        }

        private Hashtable Initialize(JObject data)
        {
            var options = new Hashtable();
            var properties = data["properties"] as JObject;

            if (properties != null)
            {
                if (properties["localization"] != null)
                    options["localization"] = GetFileString("Localization", $"{properties["localization"]}.xml");

                if (properties["formName"] != null)
                {
                    var form = GetForm(properties["formName"].ToString());
                    
                    options["form"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(form.SaveToString()));
                }                    
            }

            options["fontFamilies"] = StiWebFormHelper.GetSystemFontFamilies();

            return options;
        }

        public IActionResult Pdf(string name, string submit)
        {
            StiForm form = GetForm(name);
            var settings = new StiPdfExporterSettings
            {
                UsePdfA = false,
                ReadOnly = false
            };

            if (submit == "pdf")
            {
                var button = form.GetAllItems().FirstOrDefault(i => (i as StiFormElement)?.Name == "Button") as StiButtonElement;
                button.ButtonSendType = StiButtonSendType.PDF;
                var numberBox = form.GetAllItems().FirstOrDefault(i => (i as StiFormElement)?.Name == "OrderNumber") as StiNumberBoxElement;
                button.FormSendURL += $"?type=pdf&guid={form.Guid}&order={numberBox.Value}";

                Response.Headers.Add("Content-Type", new string[] { "application/octet-stream", "application/octet-stream", "application/download" });
                Response.Headers.Add("Content-Disposition", "attachment; filename=\"Order.pdf\"");
            }

            var content = new StiPdfExporter(settings).ExportForm(form);

            return new FileContentResult(content, "application/pdf");
        }

        public IActionResult Results(string name)
        {
            ViewData["Submits"] = Submits;
            return View("Results");
        }

        public IActionResult ResultPdf(int id) 
        {
            var content = Submits[id]["pdf"] as byte[];
            return new FileContentResult(content, "application/pdf");
        }

        public IActionResult Submit(string type, string guid, string order)
        {
            HttpRequest request = this.HttpContext.Request;
            var isFromAcrobat = request.Headers.Keys.Any(k => k.IndexOf("Acrobat") >= 0);
            request.EnableBuffering();

            byte[] buffer;
            using (BinaryReader reader = new BinaryReader(request.Body))
            {
                buffer = reader.ReadBytes(Convert.ToInt32(request.ContentLength));
            }
            
            if (type == "pdf")
            {
                var submit = new Hashtable
                {
                    ["guid"] = guid,
                    ["orderNumber"] = order,
                    ["pdf"] = buffer,
                    ["id"] = Submits.Count.ToString()
                };

                Submits.Add(submit);
            }
            else
            {
                StiPdfFormResult result = new StiPdfFormResult();
                result.ParseXFDF(buffer);
                var formGuid = result.GetValue("StiFormGuid");
                order = result.GetValue("OrderNumber");
                var tableName = "OrderItems";
                var total = result.GetTableTotalFieldValue(tableName, 2);

                var items = new List<string>();
                for (int i = 0; i < result.GetTableRowsCount(tableName); i++)
                {
                    var item = result.GetTableFieldValue(tableName, 0, i);
                    if (!string.IsNullOrEmpty(item))
                        items.Add(item);
                }

                var submit = new Hashtable
                {
                    ["guid"] = formGuid,
                    ["orderNumber"] = order,
                    ["total"] = total,
                    ["items"] = String.Join(", ", items)
                };

                Submits.Add(submit);
            }

            if (isFromAcrobat)
            {
                StiForm form = GetForm("ThankYou.mrt");
                var orderLabel = form.GetAllItems().FirstOrDefault(i => (i as StiFormElement)?.Name == "Order") as StiTextBoxElement;
                orderLabel.Text = order;

                var settings = new StiPdfExporterSettings
                {
                    UsePdfA = false,
                    ReadOnly = false
                };
                var content = new StiPdfExporter(settings).ExportForm(form);

                return new FileContentResult(content, "application/pdf");
            }
            else
            {
                return Json(new Hashtable
                {
                    ["message"] = "Thank you!"
                });
            }
        }

        private StiForm GetForm(string name)
        {
            var formStr = GetFileString("Forms", name);
            var form = new StiForm();
            form.LoadFromJson(formStr);

            var numberBox = form.GetAllItems().FirstOrDefault(i => (i as StiFormElement)?.Name == "OrderNumber") as StiNumberBoxElement;

            if (numberBox != null)
            {
                var rand = new Random();
                numberBox.Value = rand.Next(1000000);//set 'unique' number
            }
            return form;
        }

        private string GetFilePath(string folder, string fileName)
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(assemblyDirectory, folder, fileName);
        }

        private string GetFileString(string folder, string fileName)
        {
            var filePath = GetFilePath(folder, fileName);
            using (var reader = new StreamReader(filePath))
            {
                return reader.ReadToEnd();
            }
        }
    }
}