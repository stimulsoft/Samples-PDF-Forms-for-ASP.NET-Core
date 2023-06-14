using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Base.Json.Linq;
using Stimulsoft.Form;
using Stimulsoft.Form.Items;
using Stimulsoft.Form.Items.Elements;
using Stimulsoft.Form.Properties;
using Stimulsoft.Form.Web;
using System;
using System.Collections;
using System.Text;

namespace Creating_a_Form_with_Table.Controllers
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
            
            var form = CreateForm();

            var options = new Hashtable
            {
                ["form"] = Convert.ToBase64String(Encoding.UTF8.GetBytes(form.SaveToString()))
            };

            return options;
        }

        private StiForm CreateForm()
        {
            var form = new StiForm();

            var page = new StiFormPage();
            form.Pages.Add(page);

            page.Elements.Add(new StiLabelElement()
            {
                Expression = "Custom Order Form",
                ElementFont = new System.Drawing.Font("Segoe UI", 44, System.Drawing.FontStyle.Bold),
                HorizontalAlignment = StiHorizontalAlignment.Center,
                Geometry = new StiRectangleGeometry(0, 0, page.ContentAreaWidth, 200)
            }); 

            var table = new StiTableElement()
            {
                Geometry = new StiRectangleGeometry(0, 200, page.ContentAreaWidth, 500),
                ElementPadding = new StiPadding(0)
            };

            table.Columns.Add(new StiTextBoxColumnItem()
            {
                Label = "ITEM",
                HeaderAlignment = StiHorizontalAlignment.Center,
            });

            var sizeItem = new StiComboBoxColumnItem()
            {
                Label = "SIZE",
                HeaderAlignment = StiHorizontalAlignment.Center,
            };
            sizeItem.ComboBoxOptions.Add("S");
            sizeItem.ComboBoxOptions.Add("M");
            sizeItem.ComboBoxOptions.Add("L");
            table.Columns.Add(sizeItem);

            table.Columns.Add(new StiTextBoxColumnItem()
            {
                Label = "COLOR",
                HeaderAlignment = StiHorizontalAlignment.Center,
            });

            table.Columns.Add(new StiNumberBoxColumnItem()
            {
                Label = "QTY",
                HeaderAlignment = StiHorizontalAlignment.Center,
                DecimalDigits = 0,
                Width = 30,
            });

            table.Columns.Add(new StiNumberBoxColumnItem()
            {
                Label = "PRCE",
                HeaderAlignment = StiHorizontalAlignment.Center,
                Width = 60,
                UnitLabel = "$",
                UnitAlignment = StiUnitAlignment.PrefixInside
            });

            table.Columns.Add(new StiLabelColumnItem()
            {
               Label = "Total",
               HeaderAlignment = StiHorizontalAlignment.Center,
               Expression = "{Col(4) * Col(5)}",
               Prefix = "$",
               CellAlignment = StiHorizontalAlignment.Right
            });

            for (var i = 1; i <= 10; i++)            
                table.RowsCollection.Labels.Add(i.ToString());

            table.RowsCollection.Width = 20;
            table.RowsCollection.Alignment = StiHorizontalAlignment.Center;

            table.TotalsRow.Fields.Add(new StiTotalsFieldItem()
            {
                Header = "Total",
                Prefix = "$",
                Expression = "{Sum(Col(6))}"
            });

            page.Elements.Add(table);

            page.Elements.Add(new StiButtonElement()
            {
                Text = "Submit",
                ButtonType = StiButtonType.SendForm,
                Geometry = new StiRectangleGeometry(page.ContentAreaWidth / 2 - 200, 700, 400, 100)                
            });

            return form;
        }

    }
}