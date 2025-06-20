using Microsoft.AspNetCore.Mvc;
using DomainLayer.Model;
using OnionConsumeWebAPI.Extensions;
namespace CoporateBooking.Controllers.common
{
    public class BookingController : Controller
    {
        public async Task<IActionResult> MyBookingAsync()
        {
            List<Booking> bookingList = new List<Booking>();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Make GET call
                    HttpResponseMessage response = await client.GetAsync(AppUrlConstant.Getflightbooking);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        // bookingList = JsonConvert.DeserializeObject<List<Booking>>(result);

                        return View(bookingList); // Pass to Razor View
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to load booking data.";
                        return View(bookingList);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred: " + ex.Message;
                return View(bookingList);
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
