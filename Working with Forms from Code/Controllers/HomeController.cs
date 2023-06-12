using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stimulsoft.Form.Items.Elements;
using Stimulsoft.Form.Items;
using Stimulsoft.Form.Properties;
using System;
using Stimulsoft.Form.Pdf.Export;

namespace Working_with_Forms_from_Code.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        static HomeController()
        {
            // How to Activate
            //Stimulsoft.Base.StiLicense.Key = "6vJhGtLLLz2GNviWmUTrhSqnO...";
            //Stimulsoft.Base.StiLicense.LoadFromFile("license.key");
            //Stimulsoft.Base.StiLicense.LoadFromStream(stream);
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /* Forms */

        private static StiForm Form = new StiForm();

        public IActionResult Create()
        {
            var textBox = new StiTextBoxElement
            {
                Label = "Example Text Box",
                Geometry = new StiRectangleGeometry(0, 0, 400, 100).WithMaxWidth(400).WithMaxHeight(100),
                Text = "Initial Text",
            };

            var page = new StiFormPage();
            page.Elements.Add(textBox);

            HomeController.Form = new StiForm();
            HomeController.Form.Pages.Add(page);

            ViewBag.FormMessage = "A new Form has been created.";

            return View("Index");
        }

        public IActionResult Load()
        {
            var mrtFileName = "Forms\\Order.mrt";
            HomeController.Form = new StiForm();
            HomeController.Form.Load(mrtFileName);

            ViewBag.FormMessage = $"A new Form has been loaded from a file '{mrtFileName}'";

            return View("Index");
        }

        public IActionResult Save()
        {
            var mrtFileName = $"Forms\\Form_{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss")}.mrt";
            HomeController.Form.Save(mrtFileName);

            ViewBag.FormMessage = $"The Form has been saved to a file '{mrtFileName}'";

            return View("Index");
        }

        public IActionResult Export()
        {
            var exporter = new StiPdfExporter();

            var pdfFileBytes = exporter.ExportForm(HomeController.Form);
            var pdfFileName = $"Forms\\Form_{DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss")}.pdf";
            System.IO.File.WriteAllBytes(pdfFileName, pdfFileBytes);

            ViewBag.FormMessage = $"The Form has been saved to a file '{pdfFileName}'";

            return View("Index");
        }
    }
}
