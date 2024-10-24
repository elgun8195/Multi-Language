using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiLanguage.DAL;
using MultiLanguage.Models;
using System.Xml.Linq;

namespace MultiLanguage.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ResourceController : Controller
    {
        private readonly AppDbContext _context; 

        public ResourceController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();

            return View(products);  
        }
        public IActionResult UpdateProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);  
        }
        [HttpPost]
        public IActionResult UpdateProduct(int? id, Product product)
        {
            if (id == null || id != product.Id)
            {
                return BadRequest("Invalid product ID.");
            }

            string _resxFilePathTR = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.tr-TR.resx");
            string _resxFilePathEN = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.en-US.resx");
            string _resxFilePathFR = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.fr-FR.resx");

            var existingProduct = _context.Products.FirstOrDefault(x => x.Id == id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(product.Name))
            {
                existingProduct.Name = product.Name.Trim();
            }

            if (product.Price != 0 && product.Price > 0)
            {
                existingProduct.Price = product.Price;
            }

            if (!string.IsNullOrEmpty(product.Description))
            {
                existingProduct.Description = product.Description.Trim();
                UpdateResxFile(_resxFilePathEN, existingProduct.Description, product.Description.Trim());  
            }

            if (!string.IsNullOrEmpty(product.DescriptionTR))
            {
                existingProduct.DescriptionTR = product.DescriptionTR.Trim();
                UpdateResxFile(_resxFilePathTR, existingProduct.Description, product.DescriptionTR.Trim()); 
            }

            if (!string.IsNullOrEmpty(product.DescriptionFR))
            {
                existingProduct.DescriptionFR = product.DescriptionFR.Trim();
                UpdateResxFile(_resxFilePathFR, existingProduct.Description, product.DescriptionFR.Trim());  
            }
             
            _context.Products.Update(existingProduct);
            _context.SaveChanges();

            return RedirectToAction("index");
        }


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
             
            UpdateResxFile(_resxFilePathTR, description, descriptionTR);
             
            UpdateResxFile(_resxFilePathFR, description, descriptionFR);
             
            UpdateResxFile(_resxFilePathEN, description, description);
             
            _context.Products.Add(product);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        private void UpdateResxFile(string resxFilePath, string name, string value)
        {
            XDocument resxFile = XDocument.Load(resxFilePath);
                     
            var existingElement = resxFile.Root.Elements("data")
                .FirstOrDefault(e => e.Attribute("name").Value == name);

            if (existingElement != null)
            {
                existingElement.Element("value").Value = value?.Trim();  
            }
            else
            {
                resxFile.Root.Add(new XElement("data", new XAttribute("name", name?.Trim()),
                    new XElement("value", value?.Trim())));  
            }

            resxFile.Save(resxFilePath);
        }

        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
            {
                return NotFound();
            }

            string _resxFilePathTR = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.tr-TR.resx");
            string _resxFilePathEN = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.en-US.resx");
            string _resxFilePathFR = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "SharedResource.fr-FR.resx");
             
            DeleteResourceFromResx(_resxFilePathEN, product.Description);
            DeleteResourceFromResx(_resxFilePathTR, product.Description);
            DeleteResourceFromResx(_resxFilePathFR, product.Description);
 
            _context.Products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        private void DeleteResourceFromResx(string resxFilePath, string name)
        {
            XDocument resxFile = XDocument.Load(resxFilePath);

            var element = resxFile.Root.Elements("data")
                .FirstOrDefault(e => e.Attribute("name").Value == name);

            if (element != null)
            {
                element.Remove();  
                resxFile.Save(resxFilePath);
            }
        }

    }
}
