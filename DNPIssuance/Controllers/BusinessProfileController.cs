using Microsoft.AspNetCore.Mvc;

namespace Inquiry.Controllers
{
    public class BusinessProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
