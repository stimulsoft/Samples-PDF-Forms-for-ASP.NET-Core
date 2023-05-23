using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using Stimulsoft.Form.Pdf.Import.ParsePdf;
using Stimulsoft.Form.Pdf.Import.Helper;

namespace Loading_Fields_Data_from_PDF.Controllers
{
    public class HomeController : Controller
    {
        static HomeController()
        {
            // How to Activate
            //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnO...";
            //Stimulsoft.Base.StiLicense.LoadFromFile("license.key");
            //Stimulsoft.Base.StiLicense.LoadFromStream(stream);
        }

        [HttpPost]
        public IActionResult ShowPdfFieldsData(IFormFile pdfFile)
        {
            if (pdfFile != null)
                using (var stream = new MemoryStream())
                {
                    pdfFile.CopyTo(stream);
                    var bytes = stream.ToArray();
                    var document = StiPdfParser.Parse(bytes);

                    ViewData["pdfData"] = StiPdfImportDocumentHelper.ConvertData(document);
                }
            
            return View("Index");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
