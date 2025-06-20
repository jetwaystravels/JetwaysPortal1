using Microsoft.AspNetCore.Mvc;

namespace CoporateBooking.Controllers.common
{
    public class CancelBookingController : Controller
    {
        public IActionResult CancelAction()
        {
            //write cancel api



            return RedirectToAction("MyBooking", "Booking"); // ActionName, ControllerName
        }
    }
}
