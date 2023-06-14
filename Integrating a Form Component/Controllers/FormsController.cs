using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Form.Web;
using System;

namespace Integrating_a_Form_Component.Controllers
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
                        var initData = StiWebForm.Initialize(data, null);
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
       
    }
}