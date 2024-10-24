using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.DAL;
using MultiLanguage.Models;
using MultiLanguage.Services;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace MultiLanguage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private LanguageService _localization;
        public HomeController(ILogger<HomeController> logger, LanguageService localization, AppDbContext context)
        {
            _logger = logger;
            _localization = localization;
            _context = context;
        }

        public IActionResult Index()
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;

            // Veritabanından tüm ürünleri al
            List<Product> products = _context.Products.ToList();

            // Mevcut kültüre göre lokalleştirme
            foreach (var product in products)
            {
                if (currentCulture == "tr-TR")
                {
                    product.Description = GetLocalizedDescription(product, "tr-TR");
                }
                else if (currentCulture == "fr-FR")
                {
                    product.Description = GetLocalizedDescription(product, "fr-FR");
                }
                else
                {
                    product.Description = GetLocalizedDescription(product, "en-US");
                }
            }

            return View(products);
        }

        private string GetLocalizedDescription(Product product, string culture)
        {
            string resxFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", $"SharedResource.{culture}.resx");

            XDocument resxFile = XDocument.Load(resxFilePath);
            var localizedValue = resxFile.Root.Elements("data")
                .FirstOrDefault(e => e.Attribute("name").Value == product.Description)
                ?.Element("value").Value;

            return localizedValue ?? product.Description;
        }


        public IActionResult ChangeLanguage(string culture)
        { 
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7)  
                }); 
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            return Redirect(Request.Headers["Referer"].ToString());
        }



        public IActionResult Privacy()
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
             
            List<Product> products = _context.Products.ToList(); 
            foreach (var product in products)
            {
                if (currentCulture == "tr-TR")
                {
                    product.Description = GetLocalizedDescription(product, "tr-TR");
                }
                else if (currentCulture == "fr-FR")
                {
                    product.Description = GetLocalizedDescription(product, "fr-FR");
                }
                else
                {
                    product.Description = GetLocalizedDescription(product, "en-US");
                }
            }

            return View(products);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
