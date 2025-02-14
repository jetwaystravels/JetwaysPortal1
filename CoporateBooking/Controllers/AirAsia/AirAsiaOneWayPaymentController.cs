using Microsoft.AspNetCore.Mvc;

namespace OnionConsumeWebAPI.Controllers.AirAsia
{
    public class AirAsiaOneWayPaymentController : Controller
    {
        public IActionResult AirAsiaOneWayPaymentView(string GUID)
        {
            ViewBag.Guid = GUID;
            return View();
        }
    }
}
