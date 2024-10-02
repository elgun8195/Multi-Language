using Microsoft.AspNetCore.Mvc;
using MultiLanguage.DAL;
using MultiLanguage.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MultiLanguage.Controllers
{
    public class LanguageController : Controller
    {
        private readonly AppDbContext _context;

        public LanguageController(AppDbContext context)
        {
            _context = context;
        }

        // Action: Kaynak ekler
        public IActionResult AddResource()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddResource(Product product)
        {
            string _resxFilePathTR = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.tr-TR.resx");
            string _resxFilePathEN = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.en-US.resx");
            string _resxFilePathFR = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.fr-FR.resx");

            if (product == null)
            {
                return View();
            }

            // Alanları Trim() ederek boşluklardan arındırma
            var description = product.Description?.Trim();
            var descriptionTR = product.DescriptionTR?.Trim();
            var descriptionFR = product.DescriptionFR?.Trim();

            if (string.IsNullOrEmpty(description) ||
                string.IsNullOrEmpty(descriptionTR) ||
                string.IsNullOrEmpty(descriptionFR))
            {
                ModelState.AddModelError("", "Name and Value cannot be empty");
                return RedirectToAction("ListResources");
            }

            // TR Resx güncelle
            UpdateResxFile(_resxFilePathTR, description, descriptionTR);

            // FR Resx güncelle
            UpdateResxFile(_resxFilePathFR, description, descriptionFR);

            // EN Resx güncelle
            UpdateResxFile(_resxFilePathEN, description, description);

            // Product'ı veritabanına kaydet
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("index","home");
        }

        private void UpdateResxFile(string resxFilePath, string name, string value)
        {
            XDocument resxFile = XDocument.Load(resxFilePath);

            // Aynı isimde bir resource varsa, güncelle
            var existingElement = resxFile.Root.Elements("data")
                .FirstOrDefault(e => e.Attribute("name").Value == name);

            if (existingElement != null)
            {
                existingElement.Element("value").Value = value?.Trim(); // Var olan değeri güncelle ve Trim()
            }
            else
            {
                resxFile.Root.Add(new XElement("data", new XAttribute("name", name?.Trim()),
                    new XElement("value", value?.Trim()))); // Yeni değeri ekle ve Trim()
            }

            resxFile.Save(resxFilePath);
        }


        // Action: Tüm kaynakları listeler
        //public IActionResult ListResources()
        //{
        //    var resources = new List<KeyValuePair<string, string>>();

        //    // .resx dosyasını XML formatında okuyarak listele
        //    XDocument resxFile = XDocument.Load(_resxFilePath);

        //    foreach (var dataElement in resxFile.Root.Elements("data"))
        //    {
        //        var name = dataElement.Attribute("name").Value;
        //        var value = dataElement.Element("value").Value;

        //        resources.Add(new KeyValuePair<string, string>(name, value));
        //    }

        //    return View(resources); // ListResources.cshtml view'ını döndür
        //}

        //// Action: Kaynak siler
        //[HttpPost]
        //public IActionResult DeleteResource(string name)
        //{
        //    XDocument resxFile = XDocument.Load(_resxFilePath);
        //    var element = resxFile.Root.Elements("data")
        //        .FirstOrDefault(e => e.Attribute("name").Value == name);

        //    if (element != null)
        //    {
        //        element.Remove(); // Belirtilen kaynağı sil
        //        resxFile.Save(_resxFilePath);
        //    }

        //    return RedirectToAction("ListResources");
        //}
    }
}
