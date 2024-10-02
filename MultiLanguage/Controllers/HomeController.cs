using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MultiLanguage.DAL;
using MultiLanguage.Models;
using MultiLanguage.Services;
using System.Diagnostics;

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
           // ViewBag.Welcome = _localization.Getkey("Welcome").Value;
            var currentCulture = Thread.CurrentThread.CurrentCulture.Name;
            List<Product> products=_context.Products.ToList();
            return View(products);
        }

        public IActionResult ChangeLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            return Redirect(Request.Headers["Referer"].ToString());
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
