using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Form.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Editing_a_Form_in_the_Designer.Controllers
{
    [Controller]
    public class FormsController : Controller
    {
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

                    case "FormSave":
                        var formName = data["formName"].ToString();
                        SaveFileString("Forms", formName, data["form"].ToString());
                        return Json("{result: \"ok\"}");

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

        private Hashtable Initialize(JObject data)
        {
            var options = new Hashtable();
            var properties = data["properties"] as JObject;

            if (properties != null)
            {
                // Load localization file
                if (properties["localization"] != null)
                {
                    var localizationName = properties["localization"];
                    options["localization"] = GetFileString("Localization", $"{localizationName}.xml");
                }

                // Load form file
                if (properties["formName"] != null)
                {
                    var formName = properties["formName"].ToString();
                    var formContent = GetFileString("Forms", formName);
                    options["form"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(formContent));
                }
            }

            options["fontFamilies"] = StiWebFormHelper.GetSystemFontFamilies();

            return options;
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

        private void SaveFileString(string folder, string fileName, string content)
        {
            var filePath = GetFilePath(folder, fileName);
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(content);
            }
        }
    }
}