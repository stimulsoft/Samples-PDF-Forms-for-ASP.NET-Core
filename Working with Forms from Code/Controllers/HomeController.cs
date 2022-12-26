using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stimulsoft.Form.Items.Elements;
using Stimulsoft.Form.Items;
using Stimulsoft.Form.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Working_with_Forms_from_Code.Models;
using Stimulsoft.Form.Export.Pdf;

namespace Working_with_Forms_from_Code.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

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

        private StiForm Form = new StiForm();

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

            this.Form = new StiForm();
            this.Form.Pages.Add(page);

            return View();
        }

        public IActionResult Load()
        {
            this.Form = new StiForm();
            this.Form.Load("Forms\\Order.mrt");

            return View();
        }

        public IActionResult Save()
        {
            var mrtFileName = $"Forms\\Form_{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}_{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}.mrt";
            this.Form.Save(mrtFileName);

            return View();
        }

        public IActionResult Export()
        {
            var exporter = new StiPdfExporter(new StiPdfExporterSettings
            {
                UsePdfA = false,
                ReadOnly = false,
            });

            var pdfFileBytes = exporter.ExportForm(this.Form);
            var pdfFileName = $"Forms\\Form_{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}_{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}.pdf";
            System.IO.File.WriteAllBytes(pdfFileName, pdfFileBytes);

            return View();
        }
    }
}
