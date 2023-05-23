using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Form;
using Stimulsoft.Form.Items;
using Stimulsoft.Form.Items.Elements;
using Stimulsoft.Form.Web;
using System;
using System.Collections;
using System.Text;

namespace Changing_Properties_from_Code.Controllers
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
            var form = new StiForm();
            form.Load(@"Data\SoftwareEvaluationSurvey.mrt");

            form = ChangeFormProperties(form);

            var options = new Hashtable
            {
                ["form"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(form.SaveToString()))
            };

            return options;
        }

        private StiForm ChangeFormProperties(StiForm form)
        {
            var labelCompanyName = form.GetElementByName("CompanyName") as StiLabelElement;
            labelCompanyName.Expression = "My Company";
            labelCompanyName.ElementFont = new System.Drawing.Font("Arial", 20);
          
            //Save-load form just for sample
            var savedForm = form.SaveToString();

            var loadedForm = new StiForm();
            loadedForm.LoadFromString(savedForm);

            return loadedForm;
        }

    }
}