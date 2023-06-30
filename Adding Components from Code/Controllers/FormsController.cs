using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Form.Items;
using Stimulsoft.Form.Items.Elements;
using Stimulsoft.Form.Web;
using System;
using System.Collections;
using System.Text;

namespace Adding_Components_from_Code.Controllers
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
            var form = new StiForm();
            form.Load(@"Data\SoftwareEvaluationSurvey.mrt");

            AddLogoAndCompanyName(form);

            var options = new Hashtable
            {
                ["form"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(form.SaveToString()))
            };

            return options;
        }

        private void AddLogoAndCompanyName(StiForm form)
        {
            var image = new StiImageElement()
            {
                Geometry = new Stimulsoft.Form.Properties.StiRectangleGeometry(0, 0, 200, 100),
                Image = System.IO.File.ReadAllBytes(@"Data\Logo.png")
            };

            var companyName = new StiLabelElement()
            {
                Geometry = new Stimulsoft.Form.Properties.StiRectangleGeometry(0, 20, 400, 70),
            };
            companyName.Text.Expression = "Company Name";
            companyName.Text.Font = new System.Drawing.Font("Arial", 22);
            companyName.Text.HorizontalAlignment = Stimulsoft.Form.StiHorizontalAlignment.Left;

            form.Pages[0].Elements.Add(image);
            form.Pages[0].Elements.Add(companyName);
        }
      
    }
}